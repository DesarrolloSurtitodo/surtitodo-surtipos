namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases
{
    public interface IProcessPendingDocumentsUseCase
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
