namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;

public class SapInvoiceResult
{
    public bool Success { get; init; }

    public long? SapDocEntry { get; init; }

    public long? SapDocNum { get; init; }

    public int? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public int? HttpCode { get; init; }

    public string? HttpMessage { get; init; }

    public string RequestJson { get; init; } = string.Empty;

    public string ResponseJson { get; init; } = string.Empty;

}
