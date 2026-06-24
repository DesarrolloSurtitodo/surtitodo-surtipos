namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;

public interface ISapSessionManager
{
    Task<string> GetSessionAsync(CancellationToken cancellationToken);
}

