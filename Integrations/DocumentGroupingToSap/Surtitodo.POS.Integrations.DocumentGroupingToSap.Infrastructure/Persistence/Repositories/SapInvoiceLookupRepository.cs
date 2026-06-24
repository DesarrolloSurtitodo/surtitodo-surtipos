using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using System.Data.Odbc;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories;

internal class SapInvoiceLookupRepository(IOptions<HanaOptions> options) : ISapInvoiceLookupRepository
{
    private readonly string _connectionString = options.Value.ConnectionString;

    public async Task<SapInvoiceLookupResult?> FindByNumAtCardAsync(string numAtCard, CancellationToken cancellationToken)
    {
        await using var connection = new OdbcConnection(_connectionString);

        const string query = """
        SELECT
            "DocEntry",
            "DocNum"
        FROM OINV
        WHERE "NumAtCard" = ?
        """;

        await using var command = new OdbcCommand(query, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken)) return null;

        return new SapInvoiceLookupResult
        {
            DocEntry = reader.GetInt32(0),
            DocNum = reader.GetInt32(1)
        };
    }
}
