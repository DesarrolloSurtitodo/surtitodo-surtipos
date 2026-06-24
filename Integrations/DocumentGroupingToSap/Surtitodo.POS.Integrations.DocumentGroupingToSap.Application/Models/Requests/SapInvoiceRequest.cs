namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;

public class SapInvoiceRequest
{
    public string CardCode { get; init; } = string.Empty;

    public DateOnly DocDate { get; init; }

    public DateOnly DocDueDate { get; init; }

    public string NumAtCard { get; init; } = string.Empty;

    public string? Comments { get; init; }

    public int Series { get; init; }

    public List<SapInvoiceLineRequest> DocumentLines { get; init; } = new();
}
