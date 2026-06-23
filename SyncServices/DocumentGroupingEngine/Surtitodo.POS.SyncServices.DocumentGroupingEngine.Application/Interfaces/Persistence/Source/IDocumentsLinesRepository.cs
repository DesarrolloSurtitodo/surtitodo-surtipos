using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source
{
    public interface IDocumentsLinesRepository
    {
        Task<IEnumerable<DocumentsLines>> GetGroupedLinesAsync(
            IEnumerable<int> ticodiList,
            string bocodi, string cacodi, string tipdoc,
            CancellationToken ct = default);
    }
}
