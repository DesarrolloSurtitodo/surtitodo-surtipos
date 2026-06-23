using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Source.Repository
{
    public class DocumentsLinesRepository : IDocumentsLinesRepository
    {
        public async Task<IEnumerable<DocumentsLines>> GetGroupedLinesAsync(IEnumerable<int> ticodiList, string bocodi, string cacodi, 
            string tipdoc, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
