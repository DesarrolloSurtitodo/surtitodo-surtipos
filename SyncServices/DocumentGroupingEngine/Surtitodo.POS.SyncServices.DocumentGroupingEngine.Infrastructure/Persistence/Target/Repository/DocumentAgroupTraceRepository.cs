using Microsoft.EntityFrameworkCore;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target.Repository
{
    public class DocumentAgroupTraceRepository(AppDbContext ctx) : IDocumentAgroupTraceRepository
    {
        private readonly AppDbContext _ctx = ctx;

        public async Task<long?> FindAgroupIdAsync(string bocodi, string cacodi, string tipdoc, int ticodi, CancellationToken ct = default)
        {
            var trace = await _ctx.DocumentAgroupTrace
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.BOCODI == bocodi &&
                    t.CACODI == cacodi &&
                    t.TIPDOC == tipdoc &&
                    t.TICODI == ticodi, ct);

            return trace?.DocumentAgroupId;
        }

        public async Task InsertManyAsync(IEnumerable<DocumentAgroupTrace> traces, CancellationToken ct = default)
        {
            await _ctx.DocumentAgroupTrace.AddRangeAsync(traces, ct);
        }
    }
}
