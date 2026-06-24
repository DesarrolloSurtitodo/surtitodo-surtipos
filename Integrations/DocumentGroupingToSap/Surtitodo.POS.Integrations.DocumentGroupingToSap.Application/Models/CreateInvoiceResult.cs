namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models;

public sealed class CreateInvoiceResult
{
    public bool Success { get; init; }

    public int? SapDocEntry { get; init; }

    public int? SapDocNum { get; init; }

    public int? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public int? HttpCode { get; init; }

    public string? HttpMessage { get; init; }

    public string RequestJson { get; init; } = string.Empty;

    public string ResponseJson { get; init; } = string.Empty;
}