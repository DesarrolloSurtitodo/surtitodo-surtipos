using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target
{
    public interface IDocumentAgroupTraceRepository
    {
        /// <summary>
        /// Verifica si un TICODI ya fue procesado. Retorna el DocumentAgroupId si existe.
        /// </summary>
        Task<long?> FindAgroupIdAsync(string bocodi, string cacodi, string tipdoc, int ticodi, CancellationToken ct = default);

        Task InsertManyAsync(IEnumerable<DocumentAgroupTrace> traces, CancellationToken ct = default);
    }
}
