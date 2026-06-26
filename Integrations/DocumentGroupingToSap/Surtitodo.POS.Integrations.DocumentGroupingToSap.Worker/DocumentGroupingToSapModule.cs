using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application;
using Surtitodo.POS.Shared.SharedProcessMonitor;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker;

public sealed class DocumentGroupingToSapModule : IIntegrationModule
{
    private readonly IConfiguration _config;
    private IHost? _host;
    private bool _isPaused;

    public string Name => "Document Grouping → SAP";
    public string Icon => "ti-file-invoice";
    public string Description => "Integra documentos agrupados hacia SAP Business One";
    public bool IsRunning => _host is not null;
    public bool IsPaused => _isPaused;

    public WorkerEventChannel EventChannel { get; } = new();

    public DocumentGroupingToSapModule(IConfiguration config) => _config = config;

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

        await EventChannel.Writer.WriteAsync(
            new WorkerEvent(WorkerEventType.Started, "Worker iniciado.", DateTime.Now), ct);
    }

    public async Task PauseAsync(CancellationToken ct = default)
    {
        await _host!.StopAsync(ct);
        _isPaused = true;

        await EventChannel.Writer.WriteAsync(
            new WorkerEvent(WorkerEventType.Paused, "Worker pausado.", DateTime.Now), ct);
    }

    public async Task ResumeAsync(CancellationToken ct = default)
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) => RegisterServices(services, _config))
            .Build();

        await _host.StartAsync(ct);
        _isPaused = false;

        await EventChannel.Writer.WriteAsync(
            new WorkerEvent(WorkerEventType.Info, "Worker reanudado.", DateTime.Now), ct);
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (_host is not null) await _host.StopAsync(ct);
        _host = null;
        _isPaused = false;

        await EventChannel.Writer.WriteAsync(
            new WorkerEvent(WorkerEventType.Stopped, "Worker detenido.", DateTime.Now), ct);
    }
}
