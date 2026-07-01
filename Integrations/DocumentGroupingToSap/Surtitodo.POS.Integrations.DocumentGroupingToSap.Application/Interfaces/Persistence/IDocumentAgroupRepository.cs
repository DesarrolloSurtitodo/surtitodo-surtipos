using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;

public interface IDocumentAgroupRepository
{
    Task<IReadOnlyCollection<DocumentAgroup>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);

    Task MarkAsIntegrationAsync(
        long documentId,
        int? errorCode,
        string? errorMessage,
        int? httpCode,
        string? httpMessage,
        long? sapDocEntry,
        long? sapDocNum,
        string requestFile,
        string responseFile,
        string integrationStatus,
        CancellationToken cancellationToken);

    Task<(int Procesados, int Correctos, int Errores, int Pendientes)> GetMetricsAsync(CancellationToken cancellationToken);
}

