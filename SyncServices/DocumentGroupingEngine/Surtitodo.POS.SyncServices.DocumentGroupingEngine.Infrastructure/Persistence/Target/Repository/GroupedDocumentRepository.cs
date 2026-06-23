using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target.Repository
{
    public class GroupedDocumentRepository(AppDbContext ctx) : IGroupedDocumentRepository
    {
        private readonly AppDbContext _ctx = ctx;

        public async Task InsertAsync(DocumentAgroup document, CancellationToken ct = default)
        {
            await _ctx.DocumentAgroup.AddAsync(document, ct);
        }
    }
}
