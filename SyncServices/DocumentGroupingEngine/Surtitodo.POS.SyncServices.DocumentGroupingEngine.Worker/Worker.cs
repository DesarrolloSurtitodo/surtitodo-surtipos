// Worker.cs
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Pipeline;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Worker;

public class Worker : BackgroundService
{
    private readonly GroupingPipelineBehavior _pipeline;
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;

    public Worker(
        GroupingPipelineBehavior pipeline,
        ILogger<Worker> logger,
        IConfiguration config)
    {
        _pipeline = pipeline;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Intervalo configurable desde appsettings.json
        var intervalSeconds = _config.GetValue<int>("Worker:IntervalSeconds", defaultValue: 60);
        var interval = TimeSpan.FromSeconds(intervalSeconds);

        _logger.LogInformation("Worker iniciado. Intervalo: {Interval}s", intervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando ciclo de agrupación: {Time}", DateTimeOffset.Now);

            await _pipeline.HandleAsync(stoppingToken);

            _logger.LogInformation("Ciclo finalizado. Próxima ejecución en {Interval}s", intervalSeconds);

            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogInformation("Worker detenido.");
    }
}