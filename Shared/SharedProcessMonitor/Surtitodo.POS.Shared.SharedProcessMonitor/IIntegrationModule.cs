using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Surtitodo.POS.Shared.SharedProcessMonitor;

public interface IIntegrationModule
{
    string Name { get; }
    string Icon { get; }
    string Description { get; }
    bool IsRunning { get; }
    bool IsPaused { get; }

    WorkerEventChannel EventChannel { get; }

    void RegisterServices(IServiceCollection services, IConfiguration config);

    Task StartAsync(CancellationToken ct = default);
    Task PauseAsync(CancellationToken ct = default);
    Task ResumeAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
}
