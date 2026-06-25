using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using System.Data.Odbc;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories
{
    public class SapSeriesLookupRepository(IOptions<HanaOptions> options) : ISapSeriesLookupRepository
    {
        private readonly string _connectionString = options.Value.ConnectionString;

        public async Task<int?> GetSeriesNumberAsync(string warehouseCode, CancellationToken cancellationToken)
        {
            await using var connection = new OdbcConnection(_connectionString);

            const string query = """
                SELECT TOP 1
            	    T0."SeriesName",
            	    T0."Series"
                FROM "SURTITODO"."NNM1" T0
                INNER JOIN "SURTITODO"."OWHS" T1 ON SUBSTRING(T0."BeginStr", 2, 2) = RIGHT(T1."WhsCode", 2)
                WHERE
            	    T0."SeriesName" LIKE '%FEPV%' 
            	    AND T1."WhsName" LIKE '%PV%'
            	    AND T0."NextNumber" < T0."LastNum"
            	    AND T1."WhsCode" = ?
                ORDER BY "SeriesName";
            """;

            await connection.OpenAsync(cancellationToken);  // ← faltaba esto

            await using var command = new OdbcCommand(query, connection);
            command.Parameters.AddWithValue("?", warehouseCode); // ← faltaba el parámetro

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken)) return null;

            return reader.GetInt32(1);
        }
    }
}
