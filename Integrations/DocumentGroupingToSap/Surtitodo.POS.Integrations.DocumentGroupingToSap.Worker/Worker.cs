using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker
{
    public class Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger, IOptions<IntegrationOptions> options) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<Worker> _logger = logger;
        private readonly IntegrationOptions _options = options.Value;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DocumentGroupingToSap iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var useCase = scope.ServiceProvider.GetRequiredService<IProcessPendingDocumentsUseCase>();

                    await useCase.ExecuteAsync(stoppingToken);
                }
                catch (SapLoginException ex)
                {
                    _logger.LogError(
                        "Error de autenticación SAP. HTTP: {HttpCode} | SAP Code: {SapCode} | Mensaje: {SapMessage}",
                        ex.HttpStatusCode, ex.SapErrorCode, ex.SapErrorMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado ejecutando integración de documentos.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.ExecutionIntervalSeconds), stoppingToken);
            }
        }
    }
}
