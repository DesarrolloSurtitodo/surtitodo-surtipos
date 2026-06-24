namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;

public interface IJsonFileStorage
{
    Task<string> SaveRequestAsync(long documentId, string content, CancellationToken cancellationToken);

    Task<string> SaveResponseAsync(long documentId, string content, CancellationToken cancellationToken);
}
