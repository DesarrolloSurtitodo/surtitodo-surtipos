using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;
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
        ISapSeriesLookupRepository sapSeriesLookupRepository,
        ISapInvoiceService sapInvoiceService,
        IJsonFileStorage jsonFileStorage,
        IOptions<IntegrationOptions> options,
        ILogger<ProcessPendingDocumentsUseCase> logger
        ) : IProcessPendingDocumentsUseCase
    {
        private readonly IDocumentAgroupRepository _documentRepository = documentRepository;
        private readonly ISapInvoiceLookupRepository _sapInvoiceLookupRepository = sapInvoiceLookupRepository;
        private readonly ISapInvoiceService _sapInvoiceService = sapInvoiceService;
        private readonly ISapSeriesLookupRepository _sapSeriesLookupRepository = sapSeriesLookupRepository;
        private readonly IJsonFileStorage _jsonFileStorage = jsonFileStorage;
        private readonly IOptions<IntegrationOptions> _options = options;
        private readonly ILogger<ProcessPendingDocumentsUseCase> _logger = logger;

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // 1. Obtener documentos pendientes
            var documents = await _documentRepository.GetPendingAsync(_options.Value.BatchSize, cancellationToken);

            // 2. Recorrer documentos
            foreach (var document in documents)
            {
                Console.WriteLine($"Procesando documento {document.NumAtCard} de la tienda {document.WarehouseCode}");

                try
                {
                    // 3. Validar por el NumAtCard si ya existe en SAP
                    var existingInvoice = await _sapInvoiceLookupRepository.FindByNumAtCardAsync(document.NumAtCard, cancellationToken);

                    // 4. Si existe, actualizamos en la BD de agrupación
                    if (existingInvoice is not null)
                    {
                        Console.WriteLine($"Documento ya existe en SAP con DocNum {existingInvoice.DocNum} y DocEntry {existingInvoice.DocEntry}. Se procede a actualizar");

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
                                "T",
                                cancellationToken);

                        continue;
                    }

                    Console.WriteLine($"Consultando series de la tienda {document.WarehouseCode} en SAP");

                    // 5. Validar si hay series disponibles
                    int series = await _sapSeriesLookupRepository.GetSeriesNumberAsync(document.WarehouseCode, cancellationToken) 
                        ?? throw new SapSeriesNotFoundException(document.WarehouseCode, document.NumAtCard);

                    Console.WriteLine($"Series obtenida {series}");

                    // 6. Si no existe, creamos el mapping
                    Console.WriteLine($"Creando el request para el Service Layer");
                    var documentRequest = RequestDocumentInvoiceMapper.ToRequest(document, series!);


                    // 7. Crear la factura en SAP
                    Console.WriteLine($"Ingresando factura por el service layer");
                    var result = await _sapInvoiceService.CreateInvoiceAsync(documentRequest, cancellationToken);

                    // 8. Creamos los files
                    Console.WriteLine($"Creando los files de seguimiento");
                    var requestFile = await _jsonFileStorage.SaveRequestAsync(document.NumAtCard, result.RequestJson, cancellationToken);
                    var responseFile = await _jsonFileStorage.SaveResponseAsync(document.NumAtCard, result.ResponseJson, cancellationToken);


                    // 8. Actualizamos en la BD de agrupación
                    Console.WriteLine($"Actualizando en la base de datos de agrupacion");
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
                            result.Success ? "T" : "E",
                            cancellationToken);

                    Console.WriteLine();
                }
                catch (SapSeriesNotFoundException ex)
                {
                    _logger.LogWarning(ex, ex.Message);

                    await _documentRepository.MarkAsIntegrationAsync(
                        document.Id,
                        -1,
                        ex.Message,
                        500,
                        "Serie no encontrada",
                        0,
                        0,
                        string.Empty,
                        string.Empty,
                        "E",
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"Error procesando documento {NumAtCard}", document.NumAtCard);

                    await _documentRepository.MarkAsIntegrationAsync(
                        document.Id,
                        -1,
                        ex.Message[..Math.Min(ex.Message.Length, 250)],
                        500,
                        "Error inesperado",
                        0,
                        0,
                        string.Empty,
                        string.Empty,
                        "E",
                        cancellationToken);
                }
            }
        }
    }
}
