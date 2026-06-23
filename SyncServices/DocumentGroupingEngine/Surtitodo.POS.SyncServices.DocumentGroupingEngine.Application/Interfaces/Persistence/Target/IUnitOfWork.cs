namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGroupedDocumentRepository GroupedDocuments { get; }
        Task BeginAsync(CancellationToken ct = default);
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
