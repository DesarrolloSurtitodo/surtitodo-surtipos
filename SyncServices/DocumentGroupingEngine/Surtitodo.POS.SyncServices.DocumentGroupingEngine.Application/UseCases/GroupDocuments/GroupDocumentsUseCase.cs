using Microsoft.Extensions.Options;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Configuration;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Mappers;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;
using groupingEngine = Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping.DocumentGroupingEngine;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.UseCases.GroupDocuments;

public class GroupDocumentsUseCase(IDocumentsRepository documentsRepo, IDocumentsLinesRepository linesRepo, IUnitOfWork unitOfWork,
    IOptions<GroupingSettings> settings) : IGroupingOrchestrator
{
    private readonly IDocumentsRepository _documentsRepo = documentsRepo;
    private readonly IDocumentsLinesRepository _linesRepo = linesRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly GroupingSettings _settings = settings.Value;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        // 1. Leer candidatos
        var candidates = await _documentsRepo.GetCandidatesAsync(_settings.CandidatesTopLimit, ct);
        if (candidates is null) return;

        // 2. Validar trazabilidad — separa ya procesados de pendientes
        var (alreadyProcessed, pending) = await SplitByTraceAsync(candidates, ct);

        // Re-sincronizar en Source los que ya existen en trazabilidad
        foreach (var (doc, agroupId) in alreadyProcessed)
        {
            await _documentsRepo.UpdateGroupStatusAsync(
                [doc.TICODI],
                doc.BOCODI, 
                doc.CACODI, 
                doc.TIPDOC,
                statusCode: "T",
                groupedDocumentId: agroupId,
                message: "Re-sincronizado desde trazabilidad",
                logFile: null,
                ct: ct);
        }

        if (pending.Count == 0) return;

        // 3. Agrupar solo los pendientes
        var group = groupingEngine.BuildNextGroup(pending, _settings.MaxGroupAmount);
        if (group is null) return;

        // 4. Generar NumAtCard
        var numAtCard = DocumentGroupMapper.BuildNumAtCard(group.TIPDOC, group.BOCODI, group.CACODI);

        Console.WriteLine($"Procesando documento {numAtCard}");

        try
        {
            // 5. Leer líneas
            var lines = await _linesRepo.GetGroupedLinesAsync(group.MemberKeys, group.BOCODI, group.CACODI, group.TIPDOC, ct);

            // 6. Mapear — el mapper construye documento + líneas + trazas juntos
            var targetDoc = DocumentGroupMapper.ToTarget(group, lines, numAtCard);

            // 7. Persistir con atomicidad total — documento + líneas + trazas
            //    en una sola transacción. EF resuelve todas las FK por navegación.
            await _unitOfWork.BeginAsync(ct);
            await _unitOfWork.GroupedDocuments.InsertAsync(targetDoc, ct);
            await _unitOfWork.CommitAsync(ct);

            // Id disponible tras el Commit
            var groupedId = targetDoc.Id;

            // 8. Actualizar Source — fuera de la transacción Target
            await _documentsRepo.UpdateGroupStatusAsync(
                group.MemberKeys,
                group.BOCODI, 
                group.CACODI, 
                group.TIPDOC,
                statusCode: "T",
                groupedDocumentId: groupedId,
                message: "Agrupación correcta",
                logFile: null,
                ct: ct);

            Console.WriteLine($"Agrupación CORRECTA {numAtCard}. ID de agrupacion: {groupedId}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(ct);

            Console.WriteLine($"Agrupación ERROR {numAtCard}. Mensaje de error: {ex.Message}");
            Console.WriteLine();

            throw new GroupingException(
                message: ex.Message,
                numAtCard: numAtCard,
                memberKeys: group.MemberKeys,
                bocodi: group.BOCODI,
                cacodi: group.CACODI,
                tipdoc: group.TIPDOC,
                inner: ex);
        }
    }

    private async Task<(List<(Documents doc, long agroupId)> alreadyProcessed, List<Documents> pending)>SplitByTraceAsync(IEnumerable<Documents> candidates, 
        CancellationToken ct)
    {
        var alreadyProcessed = new List<(Documents, long)>();
        var pending = new List<Documents>();

        foreach (var doc in candidates)
        {
            var agroupId = await _unitOfWork.Traces.FindAgroupIdAsync(doc.BOCODI, doc.CACODI, doc.TIPDOC, doc.TICODI, ct);

            if (agroupId.HasValue)
                alreadyProcessed.Add((doc, agroupId.Value));
            else
                pending.Add(doc);
        }

        return (alreadyProcessed, pending);
    }
}