using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Mappers;
using groupingEngine = Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping.DocumentGroupingEngine;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.UseCases.GroupDocuments
{
    public class GroupDocumentsUseCase(
                    IDocumentsRepository documentsRepo,
                    IDocumentsLinesRepository linesRepo,
                    IUnitOfWork unitOfWork) : IGroupingOrchestrator
    {
        private readonly IDocumentsRepository _documentsRepo = documentsRepo;
        private readonly IDocumentsLinesRepository _linesRepo = linesRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            // 1. Leer candidatos
            var candidates = await _documentsRepo.GetCandidatesAsync(ct);

            // 2. Agrupar — lógica de dominio pura
            var group = groupingEngine.BuildNextGroup(candidates);
            if (group is null) return;

            // 3. Leer líneas agrupadas
            var lines = await _linesRepo.GetGroupedLinesAsync(group.MemberKeys, group.BOCODI, group.CACODI, group.TIPDOC, ct);

            // 4. Construir NumAtCard — el sequence viene del repositorio target
            var numAtCard = DocumentGroupMapper.BuildNumAtCard(group.TIPDOC, group.BOCODI, group.CACODI);

            // 5. Mapear
            var targetDoc = DocumentGroupMapper.ToTarget(group, lines, numAtCard);

            // 6. Persistir con atomicidad
            await _unitOfWork.BeginAsync(ct);
            var groupedId = await _unitOfWork.GroupedDocuments.InsertAsync(targetDoc, ct);
            await _unitOfWork.CommitAsync(ct);

            // 7. Actualizar Source — fuera de la transacción Target (son BDs distintas)
            await _documentsRepo.UpdateGroupStatusAsync(
                group.MemberKeys,
                group.BOCODI, group.CACODI, group.TIPDOC,
                statusCode: "A",
                groupedDocumentId: groupedId,
                message: null,
                logFile: null,
                ct: ct);
        }
    }
}
