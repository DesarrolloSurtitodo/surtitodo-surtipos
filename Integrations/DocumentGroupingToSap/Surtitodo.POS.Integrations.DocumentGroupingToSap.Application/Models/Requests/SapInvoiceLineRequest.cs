namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;

public class SapInvoiceLineRequest
{
    public string WarehouseCode { get; init; } = string.Empty;

    public string ItemCode { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal Price { get; init; }

    public decimal DiscountPercent { get; init; }

    public string TaxCode { get; init; } = string.Empty;
}
