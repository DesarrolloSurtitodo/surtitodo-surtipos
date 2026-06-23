using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target
{
    public interface IGroupedDocumentRepository
    {
        Task InsertAsync(DocumentAgroup document, CancellationToken ct = default);
    }
}
