using Microsoft.EntityFrameworkCore.Storage;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target
{
    public class UnitOfWork(AppDbContext ctx, IGroupedDocumentRepository repo, IDocumentAgroupTraceRepository traces) : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _ctx = ctx;
        private IDbContextTransaction? _tx;

        public IGroupedDocumentRepository GroupedDocuments { get; } = repo;
        public IDocumentAgroupTraceRepository Traces { get; } = traces;

        public async Task BeginAsync(CancellationToken ct = default) => _tx = await _ctx.Database.BeginTransactionAsync(ct);

        public async Task CommitAsync(CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
            await _tx!.CommitAsync(ct);
        }

        public async Task RollbackAsync(CancellationToken ct = default) => await _tx!.RollbackAsync(ct);

        public async ValueTask DisposeAsync()
        {
            if (_tx is not null) await _tx.DisposeAsync();
            await _ctx.DisposeAsync();
        }
    }
}