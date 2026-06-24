using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Mappers;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.UseCases
{
    public sealed class ProcessPendingDocumentsUseCase(
        IDocumentAgroupRepository documentRepository,
        ISapInvoiceLookupRepository sapInvoiceLookupRepository,
        ISapInvoiceService sapInvoiceService,
        IJsonFileStorage jsonFileStorage,
        IOptions<IntegrationOptions> options
        ) : IProcessPendingDocumentsUseCase
    {
        private readonly IDocumentAgroupRepository _documentRepository = documentRepository;
        private readonly ISapInvoiceLookupRepository _sapInvoiceLookupRepository = sapInvoiceLookupRepository;
        private readonly ISapInvoiceService _sapInvoiceService = sapInvoiceService;
        private readonly IJsonFileStorage _jsonFileStorage = jsonFileStorage;
        private readonly IOptions<IntegrationOptions> _options = options;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // 1. Obtener documentos pendientes
            var documents = await _documentRepository.GetPendingAsync(_options.Value.BatchSize, cancellationToken);

            // 2. Recorrer documentos
            foreach (var document in documents)
            {
                try
                {
                    // 3. Validar por el NumAtCard si ya existe en SAP
                    var existingInvoice = await _sapInvoiceLookupRepository.FindByNumAtCardAsync(document.NumAtCard, cancellationToken);

                    // 4. Si existe, actualizamos en la BD de agrupación
                    if (existingInvoice is not null)
                    {
                        await _documentRepository
                            .MarkAsIntegrationAsync(
                                document.Id,
                                0,
                                "Integración Exitosa",
                                200,
                                "OK",
                                existingInvoice.DocEntry,
                                existingInvoice.DocNum,
                                string.Empty,
                                string.Empty,
                                cancellationToken);

                        continue;
                    }

                    // 5. Si no existe, creamos el mapping
                    var documentRequest = RequestDocumentInvoiceMapper.ToRequest(document);

                    // 6. Crear la factura en SAP
                    var result = await _sapInvoiceService.CreateInvoiceAsync(documentRequest, cancellationToken);

                    // 7. Creamos los files
                    var requestFile = await _jsonFileStorage.SaveRequestAsync(document.Id, result.RequestJson, cancellationToken);
                    var responseFile = await _jsonFileStorage.SaveResponseAsync(document.Id, result.ResponseJson, cancellationToken);

                    // 8. Actualizamos en la BD de agrupación
                    await _documentRepository
                        .MarkAsIntegrationAsync(
                            document.Id,
                            result.ErrorCode,
                            result.ErrorMessage,
                            result.HttpCode,
                            result.HttpMessage,
                            result.SapDocEntry,
                            result.SapDocNum,
                            requestFile,
                            responseFile,
                            cancellationToken);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
