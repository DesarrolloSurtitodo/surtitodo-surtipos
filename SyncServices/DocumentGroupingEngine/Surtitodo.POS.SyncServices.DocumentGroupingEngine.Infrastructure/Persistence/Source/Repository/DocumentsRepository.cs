using Dapper;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;
using System.Data;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Source.Repository
{
    public class DocumentsRepository(IDbConnection connection) : IDocumentsRepository
    {
        private readonly IDbConnection _connection = connection;

        public async Task<IEnumerable<Documents>> GetCandidatesAsync(int topLimit, CancellationToken ct = default)
        {
            var sql = $@"
                SELECT TOP {topLimit}
                    T0.BOCODI,
                    T0.CACODI,
                    T0.TIPDOC,
                    T0.TICODI,
                    CAST(T0.TIDATA AS DATE) AS TIDATA,
                    T1.CardCode AS CLCODI,
                    CAST(T0.TITOT AS DECIMAL(18,2)) AS TITOT
            FROM dbo.DOCUMENTS T0
            INNER JOIN dbo.DEFAULT_CUSTOMERS T1 ON T0.BOCODI = T1.WarehouseCode
            WHERE 
                T0.AGROUP_STATUS_CODE = 'P'
            ORDER BY 
                T0.TIDATA, 
                T0.TICODI";

            var command = new CommandDefinition(sql, cancellationToken: ct);
            return await _connection.QueryAsync<Documents>(command);
        }

        public async Task<(int Procesados, int Correctos, int Errores, int Pendientes)> GetMetricsAsync(CancellationToken ct = default)
        {
            const string sql = @"
            SELECT
                AGROUP_STATUS_CODE AS StatusCode,
                COUNT(*) AS Total
            FROM dbo.DOCUMENTS
            GROUP BY AGROUP_STATUS_CODE";

            var command = new CommandDefinition(sql, cancellationToken: ct);
            var rows = await _connection.QueryAsync<(string StatusCode, int Total)>(command);

            int correctos = rows.FirstOrDefault(r => r.StatusCode == "T").Total;
            int errores = rows.FirstOrDefault(r => r.StatusCode == "E").Total;
            int pendientes = rows.FirstOrDefault(r => r.StatusCode == "P").Total;
            int procesados = correctos + errores;

            return (procesados, correctos, errores, pendientes);
        }

        public async Task UpdateGroupStatusAsync(IEnumerable<int> ticodiList, string bocodi, string cacodi, string tipdoc, string statusCode, long? groupedDocumentId, 
            string? message, string? logFile, CancellationToken ct = default)
        {
            if (!ticodiList.Any())
                return;

            const string sql = @"
            UPDATE dbo.DOCUMENTS
            SET
                AGROUP_STATUS_CODE = @StatusCode,
                AGROUP_DATE = CAST(GETDATE() AS DATE),
                AGROUP_HOUR = CAST(GETDATE() AS TIME),
                AGROUP_DATETIME = GETDATE(),
                AGROUP_DOCUMENT_ID = @GroupedDocumentId,
                AGROUP_MESSAGE = @Message,
                AGROUP_LOG_ERROR_FILE = @LogFile
            WHERE
                BOCODI = @BOCODI
                AND CACODI = @CACODI
                AND TIPDOC = @TIPDOC
                AND TICODI IN @TICODI_LIST;";

            var command = new CommandDefinition(
                sql,
                new
                {
                    StatusCode = statusCode,
                    GroupedDocumentId = groupedDocumentId,
                    Message = message,
                    LogFile = logFile,
                    BOCODI = bocodi,
                    CACODI = cacodi,
                    TIPDOC = tipdoc,
                    TICODI_LIST = ticodiList
                },
                cancellationToken: ct);

            await _connection.ExecuteAsync(command);
        }
    }
}
