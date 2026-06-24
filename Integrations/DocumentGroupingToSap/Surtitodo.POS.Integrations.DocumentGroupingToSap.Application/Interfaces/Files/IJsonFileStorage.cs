namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;

public interface IJsonFileStorage
{
    Task<string> SaveRequestAsync(string numAtCard, string content, CancellationToken cancellationToken);

    Task<string> SaveResponseAsync(string numAtCard, string content, CancellationToken cancellationToken);
}
