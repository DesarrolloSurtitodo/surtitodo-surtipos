namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services
{
    public interface IGroupingOrchestrator
    {
        Task ExecuteAsync(CancellationToken ct = default);
    }
}
