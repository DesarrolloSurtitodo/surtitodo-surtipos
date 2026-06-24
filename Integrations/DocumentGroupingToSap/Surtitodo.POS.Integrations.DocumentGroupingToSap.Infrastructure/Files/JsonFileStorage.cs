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

    public async Task<string> SaveRequestAsync(string numAtCard, string content, CancellationToken cancellationToken)
    {
        return await SaveAsync(_options.RequestsPath, numAtCard, content, cancellationToken);
    }

    public async Task<string> SaveResponseAsync(string numAtCard, string content, CancellationToken cancellationToken)
    {
        return await SaveAsync(_options.ResponsesPath, numAtCard, content, cancellationToken);
    }

    private static async Task<string> SaveAsync(string basePath, string numAtCard, string content, CancellationToken cancellationToken)
    {
        // Resolver siempre relativo al ejecutable, igual que Serilog
        var absolutePath = Path.IsPathRooted(basePath)
            ? basePath
            : Path.Combine(AppContext.BaseDirectory, basePath);

        Directory.CreateDirectory(absolutePath);

        var fileName = $"{numAtCard}.json";
        var filePath = Path.Combine(absolutePath, $"{numAtCard}.json");

        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken);

        return fileName;
    }
}