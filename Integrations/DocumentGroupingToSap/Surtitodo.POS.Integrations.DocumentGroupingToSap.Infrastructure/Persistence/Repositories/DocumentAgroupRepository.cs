using Microsoft.EntityFrameworkCore;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Services;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Context;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories
{
    public class DocumentAgroupRepository(SapIntegrationDbContext context, IDateTimeProvider dateTimeProvider) : IDocumentAgroupRepository
    {
        private readonly SapIntegrationDbContext _context = context;
        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

        public async Task<IReadOnlyCollection<DocumentAgroup>> GetPendingAsync(int batchSize, CancellationToken cancellationToken)
        {
            return await _context.DocumentAgroups
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.IntegrationStatus == "P")
                .OrderBy(x => x.Id)
                .Take(batchSize)
                .ToListAsync(cancellationToken);
        }

        public async Task MarkAsErrorAsync(long documentId, int? errorCode, string? errorMessage, int? httpCode, string? httpMessage, string requestFile, string responseFile, 
            CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.Now;
            var document = await _context.DocumentAgroups.FirstAsync(x => x.Id == documentId, cancellationToken);

            document.IntegrationStatus = "E";
            document.IntegrationDate = DateOnly.FromDateTime(now);
            document.IntegrationHour = TimeOnly.FromDateTime(now);
            document.IntegrationDateTime = now;
            document.IntegrationCode = errorCode;
            document.IntegrationMessage = errorMessage;
            document.IntegrationHttpCode = httpCode;
            document.IntegrationHttpMessage = httpMessage;
            document.IntegrationJsonRequestFile = requestFile;
            document.IntegrationJsonResponseFile = responseFile;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsTransferredAsync(long documentId, int? errorCode, string? errorMessage, int? httpCode, string? httpMessage, int sapDocEntry, int sapDocNum, 
            string requestFile, string responseFile, CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.Now;
            var document = await _context.DocumentAgroups.FirstAsync(x => x.Id == documentId, cancellationToken);

            document.IntegrationStatus = "T";
            document.IntegrationDate = DateOnly.FromDateTime(now);
            document.IntegrationHour = TimeOnly.FromDateTime(now);
            document.IntegrationDateTime = now;
            document.IntegrationCode = errorCode;
            document.IntegrationMessage = errorMessage;
            document.IntegrationHttpCode = httpCode;
            document.IntegrationHttpMessage = httpMessage;
            document.IntegrationJsonRequestFile = requestFile;
            document.IntegrationJsonResponseFile = responseFile;
            document.DocEntrySap = sapDocEntry;
            document.DocNumSap = sapDocNum;

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
