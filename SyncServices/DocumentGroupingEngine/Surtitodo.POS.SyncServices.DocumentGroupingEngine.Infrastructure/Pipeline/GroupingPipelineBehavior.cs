using Microsoft.Extensions.Logging;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Pipeline
{
    public class GroupingPipelineBehavior(
                IGroupingOrchestrator useCase,
                IDocumentsRepository documentsRepo,
                IErrorLogService errorLog,
                ILogger<GroupingPipelineBehavior> logger)
    {
        private readonly IGroupingOrchestrator _useCase = useCase;
        private readonly IDocumentsRepository _documentsRepo = documentsRepo;
        private readonly IErrorLogService _errorLog = errorLog;
        private readonly ILogger<GroupingPipelineBehavior> _logger = logger;

        public async Task HandleAsync(CancellationToken ct = default)
        {
            try
            {
                await _useCase.ExecuteAsync(ct);
            }
            catch (GroupingException ex)
            {
                _logger.LogError(ex, "Error en agrupación {NumAtCard}", ex.NumAtCard);

                var logFile = await _errorLog.LogErrorAsync(ex.NumAtCard, ex, ct);

                await _documentsRepo.UpdateGroupStatusAsync(
                    ex.MemberKeys,
                    ex.Bocodi, 
                    ex.Cacodi, 
                    ex.Tipdoc,
                    statusCode: "E",
                    groupedDocumentId: null,
                    message: ex.InnerException?.Message ?? ex.Message,
                    logFile: logFile,
                    ct: ct);
            }
        }
    }
}
