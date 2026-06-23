using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Pipeline;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Worker;

public class Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger, IConfiguration config) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<Worker> _logger = logger;
    private readonly IConfiguration _config = config;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _config.GetValue<int>("Worker:IntervalSeconds", 60);
        var interval = TimeSpan.FromSeconds(intervalSeconds);

        _logger.LogInformation("Worker iniciado. Intervalo: {Interval}s", intervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Iniciando ciclo");

                await using var scope = _scopeFactory.CreateAsyncScope();

                var pipeline = scope.ServiceProvider
                    .GetRequiredService<GroupingPipelineBehavior>();

                await pipeline.HandleAsync(stoppingToken);

                _logger.LogInformation("Ciclo finalizado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo del worker");
            }

            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogInformation("Worker detenido.");
    }
}