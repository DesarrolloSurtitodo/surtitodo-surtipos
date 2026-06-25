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
            "DocNum"
        FROM "SURTITODO"."OINV"
        WHERE "NumAtCard" = ?
        """;

        await connection.OpenAsync(cancellationToken);  // ← faltaba esto

        await using var command = new OdbcCommand(query, connection);
        command.Parameters.AddWithValue("?", numAtCard); // ← faltaba el parámetro

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken)) return null;

        return new SapInvoiceLookupResult
        {
            DocEntry = reader.GetInt64(0),  // ← long
            DocNum = reader.GetInt64(1)   // ← long
        };
    }
}
