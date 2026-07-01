using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Pipeline;
using Surtitodo.POS.Shared.SharedProcessMonitor;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Worker;

public class Worker(
    IServiceScopeFactory scopeFactory,
    ILogger<Worker> logger,
    IConfiguration config,
    WorkerEventChannel eventChannel) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IConfiguration _config = config;
    private readonly WorkerEventChannel _eventChannel = eventChannel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _config.GetValue<int>("Worker:IntervalSeconds", 60);
        var interval = TimeSpan.FromSeconds(intervalSeconds);

        _logger.LogInformation("Worker iniciado. Intervalo: {Interval}s", intervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Emit(WorkerEventType.Info, "Iniciando ciclo de agrupación...", stoppingToken);

                await using var scope = _scopeFactory.CreateAsyncScope();

                var pipeline = scope.ServiceProvider
                    .GetRequiredService<GroupingPipelineBehavior>();

                await pipeline.HandleAsync(stoppingToken);

                await Emit(WorkerEventType.Info, "Ciclo finalizado.", stoppingToken);

                // ── Métricas ──────────────────────────────────────────────────
                var docsRepo = scope.ServiceProvider.GetRequiredService<IDocumentsRepository>();
                var (procesados, correctos, errores, pendientes) =
                    await docsRepo.GetMetricsAsync(stoppingToken);

                await _eventChannel.Writer.WriteAsync(new WorkerEvent(
                    WorkerEventType.Metrics,
                    "Métricas actualizadas",
                    DateTime.Now,
                    null,
                    procesados, correctos, errores, pendientes), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo del worker");
                await Emit(WorkerEventType.Error, $"Error en ciclo: {ex.Message}", stoppingToken);
            }

            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogInformation("Worker detenido.");
    }

    private async Task Emit(WorkerEventType type, string message, CancellationToken ct)
    {
        try
        {
            await _eventChannel.Writer.WriteAsync(
                new WorkerEvent(type, message, DateTime.Now), ct);
        }
        catch { }
    }
}