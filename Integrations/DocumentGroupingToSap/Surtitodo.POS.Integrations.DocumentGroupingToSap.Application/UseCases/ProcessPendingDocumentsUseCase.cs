using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Mappers;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using Surtitodo.POS.Shared.SharedProcessMonitor;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.UseCases
{
    public sealed class ProcessPendingDocumentsUseCase(
        IDocumentAgroupRepository documentRepository,
        ISapInvoiceLookupRepository sapInvoiceLookupRepository,
        ISapSeriesLookupRepository sapSeriesLookupRepository,
        ISapInvoiceService sapInvoiceService,
        IJsonFileStorage jsonFileStorage,
        IOptions<IntegrationOptions> options,
        ILogger<ProcessPendingDocumentsUseCase> logger,
        WorkerEventChannel eventChannel
        ) : IProcessPendingDocumentsUseCase
    {
        private readonly IDocumentAgroupRepository _documentRepository = documentRepository;
        private readonly ISapInvoiceLookupRepository _sapInvoiceLookupRepository = sapInvoiceLookupRepository;
        private readonly ISapInvoiceService _sapInvoiceService = sapInvoiceService;
        private readonly ISapSeriesLookupRepository _sapSeriesLookupRepository = sapSeriesLookupRepository;
        private readonly IJsonFileStorage _jsonFileStorage = jsonFileStorage;
        private readonly IOptions<IntegrationOptions> _options = options;
        private readonly ILogger<ProcessPendingDocumentsUseCase> _logger = logger;
        private readonly WorkerEventChannel _eventChannel = eventChannel;

        private async Task Emit(WorkerEventType type, string message, string? numAtCard = null)
        {
            try
            {
                await _eventChannel.Writer.WriteAsync(
                    new WorkerEvent(type, message, DateTime.Now, numAtCard));
            }
            catch { }
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // 1. Obtener documentos pendientes
            var documents = await _documentRepository.GetPendingAsync(_options.Value.BatchSize, cancellationToken);

            // 2. Recorrer documentos
            foreach (var document in documents)
            {
                await Emit(WorkerEventType.Info, $"Procesando {document.NumAtCard}", document.NumAtCard);

                try
                {
                    // 3. Validar por el NumAtCard si ya existe en SAP
                    var existingInvoice = await _sapInvoiceLookupRepository.FindByNumAtCardAsync(document.NumAtCard, cancellationToken);

                    // 4. Si existe, actualizamos en la BD de agrupación
                    if (existingInvoice is not null)
                    {
                        await Emit(WorkerEventType.Warning, $"Ya existe en SAP — DocNum: {existingInvoice.DocNum} / DocEntry: {existingInvoice.DocEntry}", document.NumAtCard);

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
                   
                    await Emit(WorkerEventType.Info, $"Consultando series tienda {301 /*document.WarehouseCode */}", document.NumAtCard);

                    // 5. Validar si hay series disponibles
                    int series = await _sapSeriesLookupRepository.GetSeriesNumberAsync("301" /*document.WarehouseCode */, cancellationToken) 
                        ?? throw new SapSeriesNotFoundException(document.WarehouseCode, document.NumAtCard);

                    await Emit(WorkerEventType.Info, $"Series obtenida: {series}", document.NumAtCard);

                    // 6. Si no existe, creamos el mapping
                    await Emit(WorkerEventType.Info, "Creando request para Service Layer", document.NumAtCard);
                    var documentRequest = RequestDocumentInvoiceMapper.ToRequest(document, series!);


                    // 7. Crear la factura en SAP
                    await Emit(WorkerEventType.Info, "Enviando factura al Service Layer...", document.NumAtCard);
                    var result = await _sapInvoiceService.CreateInvoiceAsync(documentRequest, cancellationToken);

                    if (result.Success)
                        await Emit(WorkerEventType.Success, $"Factura creada — DocEntry: {result.SapDocEntry} / DocNum: {result.SapDocNum}", document.NumAtCard);
                    else
                        await Emit(WorkerEventType.Error, $"SAP rechazó la factura: {result.ErrorMessage}", document.NumAtCard);

                    // 8. Creamos los files
                    await Emit(WorkerEventType.Info, "Guardando archivos de seguimiento", document.NumAtCard);
                    var requestFile = await _jsonFileStorage.SaveRequestAsync(document.NumAtCard, result.RequestJson, cancellationToken);
                    var responseFile = await _jsonFileStorage.SaveResponseAsync(document.NumAtCard, result.ResponseJson, cancellationToken);


                    // 8. Actualizamos en la BD de agrupación
                    await Emit(WorkerEventType.Info, "Actualizando estado en BD", document.NumAtCard);
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

                    await Emit(WorkerEventType.Error, $"Serie no encontrada para tienda {document.WarehouseCode}", document.NumAtCard);

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

                    await Emit(WorkerEventType.Error, ex.Message[..Math.Min(ex.Message.Length, 120)], document.NumAtCard);

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
