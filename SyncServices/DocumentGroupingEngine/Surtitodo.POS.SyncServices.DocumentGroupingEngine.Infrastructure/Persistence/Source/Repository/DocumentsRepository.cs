using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Source.Repository
{
    public class DocumentsRepository : IDocumentsRepository
    {
        public async Task<IEnumerable<Documents>> GetCandidatesAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateGroupStatusAsync(IEnumerable<int> ticodiList, string bocodi, string cacodi, string tipdoc, string statusCode, long? groupedDocumentId, 
            string? message, string? logFile, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
