using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using System.Data.Odbc;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories;

public class SapInvoiceLookupRepository(IOptions<HanaOptions> options) : ISapInvoiceLookupRepository
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public async Task<SapInvoiceLookupResult?> FindByNumAtCardAsync(string numAtCard, CancellationToken cancellationToken)
    {
        await using var connection = new OdbcConnection(_connectionString);

        const string query = """
            SELECT
                "DocEntry",
                "DocNum",
                'OINV' AS "DocType"
            FROM "SURTITODO"."OINV"
            WHERE "NumAtCard" = ?

            UNION ALL

            SELECT
                "DocEntry",
                "DocNum",
                'ORIN' AS "DocType"
            FROM "SURTITODO"."ORIN"
            WHERE "NumAtCard" = ?
            """;

        await connection.OpenAsync(cancellationToken);  // ← faltaba esto

        await using var command = new OdbcCommand(query, connection);
        command.Parameters.AddWithValue("?", numAtCard); // param 1 → OINV
        command.Parameters.AddWithValue("?", numAtCard); // param 2 → ORIN

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken)) return null;

        return new SapInvoiceLookupResult
        {
            DocEntry = reader.GetInt64(0),  // ← long
            DocNum = reader.GetInt64(1)   // ← long
        };
    }
}
