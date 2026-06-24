using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using System.Text;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Files;

/// <summary>
/// Almacena los JSON de solicitud y respuesta intercambiados con SAP.
/// </summary>
public sealed class JsonFileStorage(IOptions<StorageOptions> options) : IJsonFileStorage
{
    private readonly StorageOptions _options = options.Value;

    public async Task<string> SaveRequestAsync(long documentId, string content, CancellationToken cancellationToken)
    {
        return await SaveAsync(_options.RequestsPath, documentId, content, cancellationToken);
    }

    public async Task<string> SaveResponseAsync(long documentId, string content, CancellationToken cancellationToken)
    {
        return await SaveAsync(_options.ResponsesPath, documentId, content, cancellationToken);
    }

    private static async Task<string> SaveAsync(string basePath, long documentId, string content, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(basePath);

        var filePath = Path.Combine(basePath, $"{documentId}.json");

        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken);

        return filePath;
    }
}