using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Mappers;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping;
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
            if (candidates is null) return;

            // 2. Agrupar — lógica de dominio pura
            var group = groupingEngine.BuildNextGroup(candidates);
            if (group is null) return;

            // 3. Generar NumAtCard antes de cualquier operación riesgosa
            var numAtCard = DocumentGroupMapper.BuildNumAtCard(group.TIPDOC, group.BOCODI, group.CACODI);

            try
            {
                // 4. Leer líneas del grupo
                var lines = await _linesRepo.GetGroupedLinesAsync(group.MemberKeys, group.BOCODI, group.CACODI, group.TIPDOC, ct);

                // 5. Mapear Source → Target
                var targetDoc = DocumentGroupMapper.ToTarget(group, lines, numAtCard);

                // 6. Persistir con atomicidad
                await _unitOfWork.BeginAsync(ct);

                // AddAsync trackea la entidad pero aún no va a BD
                await _unitOfWork.GroupedDocuments.InsertAsync(targetDoc, ct);

                // SaveChanges + Commit → EF resuelve el Id
                await _unitOfWork.CommitAsync(ct);

                // Ahora sí el Id está disponible en la entidad trackeada
                var groupedId = targetDoc.Id;

                // 7. Actualizar Source — fuera de la transacción Target
                await _documentsRepo.UpdateGroupStatusAsync(
                    group.MemberKeys,
                    group.BOCODI, 
                    group.CACODI, 
                    group.TIPDOC,
                    statusCode: "T",
                    groupedDocumentId: groupedId,
                    message: null,
                    logFile: null,
                    ct: ct);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(ct);

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
