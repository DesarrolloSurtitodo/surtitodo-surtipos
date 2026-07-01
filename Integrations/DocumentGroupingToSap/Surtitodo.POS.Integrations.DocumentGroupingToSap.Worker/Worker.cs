// Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker/Worker.cs

using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using Surtitodo.POS.Shared.SharedProcessMonitor;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker;

public class Worker(
    IServiceScopeFactory scopeFactory,
    ILogger<Worker> logger,
    IOptions<IntegrationOptions> options,
    WorkerEventChannel eventChannel) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IntegrationOptions _options = options.Value;
    private readonly WorkerEventChannel _eventChannel = eventChannel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DocumentGroupingToSap iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Emit(WorkerEventType.Info,
                    "Iniciando ciclo — verificando documentos pendientes...", stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var useCase = scope.ServiceProvider
                    .GetRequiredService<IProcessPendingDocumentsUseCase>();

                await useCase.ExecuteAsync(stoppingToken);

                // Leer métricas y emitirlas
                using var metricScope = _scopeFactory.CreateScope();
                var repo = metricScope.ServiceProvider
                    .GetRequiredService<IDocumentAgroupRepository>();

                var (procesados, correctos, errores, pendientes) =
                    await repo.GetMetricsAsync(stoppingToken);

                await _eventChannel.Writer.WriteAsync(new WorkerEvent(
                    WorkerEventType.Metrics,
                    "Métricas actualizadas",
                    DateTime.Now,
                    null,
                    procesados, correctos, errores, pendientes), stoppingToken);

                await Emit(WorkerEventType.Info,
                    "Ciclo completado. Esperando próxima ejecución...", stoppingToken);
            }
            catch (SapLoginException ex)
            {
                _logger.LogError(
                    "Error de autenticación SAP. HTTP: {H} | Code: {C} | Mensaje: {M}",
                    ex.HttpStatusCode, ex.SapErrorCode, ex.SapErrorMessage);

                await Emit(WorkerEventType.Error,
                    $"Error de autenticación SAP: {ex.SapErrorMessage}", stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado ejecutando integración.");
                await Emit(WorkerEventType.Error, $"Error inesperado: {ex.Message}", stoppingToken);
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_options.ExecutionIntervalSeconds), stoppingToken);
        }
    }

    private async Task Emit(WorkerEventType type, string message, CancellationToken ct)
    {
        try
        {
            await _eventChannel.Writer.WriteAsync(
                new WorkerEvent(type, message, DateTime.Now), ct);
        }
        catch { /* canal cerrado — ignorar */ }
    }
}