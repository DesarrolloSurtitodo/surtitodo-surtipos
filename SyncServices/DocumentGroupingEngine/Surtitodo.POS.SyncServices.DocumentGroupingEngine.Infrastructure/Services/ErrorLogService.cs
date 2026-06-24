using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Services
{
    public class ErrorLogService(ILogger<ErrorLogService> logger, IConfiguration config) : IErrorLogService
    {
        private readonly ILogger<ErrorLogService> _logger = logger;
        private readonly string _logBasePath = config["Paths:ErrorLogPath"] ?? "logs/errors";

        public async Task<string> LogErrorAsync(
            string numAtCard,
            Exception exception,
            CancellationToken ct = default)
        {
            var now = DateTime.Now;

            var fileName = $"{now:yyyy-MM-dd}_{now:HH-mm-ss}_{numAtCard}.log";
            var fullPath = Path.Combine(_logBasePath, fileName);

            Directory.CreateDirectory(_logBasePath);

            var content = $"""
                Fecha: {now:yyyy-MM-dd HH:mm:ss}
                NumAtCard: {numAtCard}

                {exception}
            """;

            await File.WriteAllTextAsync(fullPath, content, ct);

            _logger.LogError(exception, "Log de error generado: {FileName}", fileName);

            return fileName;
        }
    }
}
