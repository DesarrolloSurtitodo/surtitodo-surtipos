using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;

public interface IDocumentAgroupRepository
{
    Task<IReadOnlyCollection<DocumentAgroup>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);

    Task MarkAsProcessingAsync(IEnumerable<long> documentIds, CancellationToken cancellationToken);

    Task MarkAsTransferredAsync(
        long documentId,
        int? errorCode,
        string? errorMessage,
        int? httpCode,
        string? httpMessage,
        int sapDocEntry,
        int sapDocNum,
        string requestFile,
        string responseFile,
        CancellationToken cancellationToken);

    Task MarkAsErrorAsync(
        long documentId,
        int? errorCode,
        string? errorMessage,
        int? httpCode,
        string? httpMessage,
        string responseFile,
        CancellationToken cancellationToken);
}

