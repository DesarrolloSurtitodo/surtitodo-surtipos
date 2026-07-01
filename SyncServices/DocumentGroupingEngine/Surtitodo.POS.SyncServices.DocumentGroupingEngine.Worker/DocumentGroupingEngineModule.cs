using Surtitodo.POS.Shared.SharedProcessMonitor;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Worker;

public sealed class DocumentGroupingEngineModule(IConfiguration config) : IIntegrationModule
{
    private readonly IConfiguration _config = config;
    private IHost? _host;
    private bool _isPaused;

    public string Name => "Document Grouping Engine";
    public string Icon => "ti-layers-intersect";
    public string Description => "Agrupa documentos de venta pendientes desde el POS origen";
    public bool IsRunning => _host is not null;
    public bool IsPaused => _isPaused;

    public int IntervalSeconds => int.TryParse(_config["Worker:IntervalSeconds"], out var s) ? s : 60;

    public WorkerEventChannel EventChannel { get; } = new();

    public void RegisterServices(IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton(EventChannel);
        services.AddApplication(config);
        services.AddInfrastructure(config);
        services.AddHostedService<Worker>();
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => RegisterServices(services, _config))
            .Build();

        await _host.StartAsync(ct);
        _isPaused = false;

        await EventChannel.Writer.WriteAsync(new WorkerEvent(WorkerEventType.Started, "Worker iniciado.", DateTime.Now), ct);
    }

    public async Task PauseAsync(CancellationToken ct = default)
    {
        await _host!.StopAsync(ct);
        _isPaused = true;

        await EventChannel.Writer.WriteAsync(new WorkerEvent(WorkerEventType.Paused, "Worker pausado.", DateTime.Now), ct);
    }

    public async Task ResumeAsync(CancellationToken ct = default)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => RegisterServices(services, _config))
            .Build();

        await _host.StartAsync(ct);
        _isPaused = false;

        await EventChannel.Writer.WriteAsync(new WorkerEvent(WorkerEventType.Info, "Worker reanudado.", DateTime.Now), ct);
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (_host is not null) await _host.StopAsync(ct);
        _host = null;
        _isPaused = false;

        await EventChannel.Writer.WriteAsync(new WorkerEvent(WorkerEventType.Stopped, "Worker detenido.", DateTime.Now), ct);
    }
}
