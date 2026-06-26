using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Core.Contracts;

interface ISyncModule
{
    string Id { get; }

    string Name { get; }

    string Description { get; }

    ModuleStatus Status { get; }

    ModuleStatistics Statistics { get; }

    Task StartAsync();

    Task StopAsync();

    Task PauseAsync();

    Task ResumeAsync();
}
