using Dapper;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;
using System.Data;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Source.Repository
{
    public class DocumentsLinesRepository(IDbConnection connection) : IDocumentsLinesRepository
    {
        private readonly IDbConnection _connection = connection;

        public async Task<IEnumerable<DocumentsLines>> GetGroupedLinesAsync(IEnumerable<int> ticodiList, string bocodi, string cacodi, 
            string tipdoc, CancellationToken ct = default)
        {
            const string sql = @"
            SELECT
                T1.BOCODI,
                T1.CACODI,
                T1.TIPDOC,
                T1.ARCODI,
                SUM(CAST(T1.TLQTT AS INT)) AS TLQTT,
                SUM(CAST(T1.TLTOT AS DECIMAL(18,2))) AS TLTOT,
                SUM(CAST(T1.TLPDTE AS INT)) AS TLPDTE,
                T1.CODIGO_IVA
            FROM dbo.DOCUMENTS_LINES T1
            WHERE
                T1.BOCODI = @BOCODI
                AND T1.CACODI = @CACODI
                AND T1.TIPDOC = @TIPDOC
                AND T1.TICODI IN @TICODI_LIST
            GROUP BY
                T1.BOCODI,
                T1.CACODI,
                T1.TIPDOC,
                T1.ARCODI,
                T1.CODIGO_IVA;";

            var command = new CommandDefinition(
                sql,
                new
                {
                    BOCODI = bocodi,
                    CACODI = cacodi,
                    TIPDOC = tipdoc,
                    TICODI_LIST = ticodiList
                },
                cancellationToken: ct);

            return await _connection.QueryAsync<DocumentsLines>(command);
        }
    }
}
