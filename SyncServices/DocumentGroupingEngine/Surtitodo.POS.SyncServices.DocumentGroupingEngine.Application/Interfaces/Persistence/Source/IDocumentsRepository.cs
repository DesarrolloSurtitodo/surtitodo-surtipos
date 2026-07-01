using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source
{
    public interface IDocumentsRepository
    {
        Task<IEnumerable<Documents>> GetCandidatesAsync(int topLimit, CancellationToken ct = default);
        Task UpdateGroupStatusAsync(
            IEnumerable<int> ticodiList,
            string bocodi, string cacodi, string tipdoc,
            string statusCode, long? groupedDocumentId,
            string? message, string? logFile,
            CancellationToken ct = default);

        Task<(int Procesados, int Correctos, int Errores, int Pendientes)> GetMetricsAsync(CancellationToken ct = default);
    }
}
