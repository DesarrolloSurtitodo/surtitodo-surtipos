using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Mappers;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;
using groupingEngine = Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping.DocumentGroupingEngine;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.UseCases.GroupDocuments
{
    public class GroupDocumentsUseCase(IDocumentsRepository documentsRepo, IDocumentsLinesRepository linesRepo, IUnitOfWork unitOfWork) : IGroupingOrchestrator
    {
        private readonly IDocumentsRepository _documentsRepo = documentsRepo;
        private readonly IDocumentsLinesRepository _linesRepo = linesRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        // Método auxiliar privado
        private async Task<(List<(Documents doc, long agroupId)> alreadyProcessed, List<Documents> pending)> SplitByTraceAsync(IEnumerable<Documents> candidates, 
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

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            // 1. Leer candidatos
            var candidates = await _documentsRepo.GetCandidatesAsync(ct);
            if (candidates is null) return;

            // 2. Validar trazabilidad ANTES de agrupar
            //    Separa los que ya fueron procesados de los que no
            var (alreadyProcessed, pending) = await SplitByTraceAsync(candidates, ct);

            // Si hay candidatos ya procesados pero sin actualizar en Source → sincronizar
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
            var group = groupingEngine.BuildNextGroup(pending);
            if (group is null) return;

            // 4. Generar NumAtCard
            var numAtCard = DocumentGroupMapper.BuildNumAtCard(group.TIPDOC, group.BOCODI, group.CACODI);

            Console.WriteLine($"Procesando documento {numAtCard}");

            try
            {
                // 5. Leer líneas
                var lines = await _linesRepo.GetGroupedLinesAsync(group.MemberKeys, group.BOCODI, group.CACODI, group.TIPDOC, ct);

                // 6. Mapear
                var targetDoc = DocumentGroupMapper.ToTarget(group, lines, numAtCard);

                // 7. Persistir con atomicidad — documento + trazas en la misma transacción
                await _unitOfWork.BeginAsync(ct);

                await _unitOfWork.GroupedDocuments.InsertAsync(targetDoc, ct);
                await _unitOfWork.CommitAsync(ct);  // ← aquí EF resuelve targetDoc.Id

                // Ahora sí el Id está disponible en la entidad trackeada
                var groupedId = targetDoc.Id;

                // Insertar trazas con el Id ya resuelto
                var traces = group.Members.Select(m => new DocumentAgroupTrace
                {
                    DocumentAgroupId = groupedId,
                    BOCODI = m.BOCODI,
                    CACODI = m.CACODI,
                    TIPDOC = m.TIPDOC,
                    TICODI = m.TICODI,
                    TracedAt = DateTime.Now
                });

                // Las trazas van en la misma transacción que el documento
                await _unitOfWork.BeginAsync(ct);
                await _unitOfWork.Traces.InsertManyAsync(traces, ct);
                await _unitOfWork.CommitAsync(ct);


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
    }
}
