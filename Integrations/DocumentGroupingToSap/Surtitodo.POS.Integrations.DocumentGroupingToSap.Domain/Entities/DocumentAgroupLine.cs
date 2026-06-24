namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

public class DocumentAgroupLine
{
    public long Id { get; set; }

    public long DocumentAgroupId { get; set; }

    public string WarehouseCode { get; set; } = string.Empty;

    public string ItemCode { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int DiscountPercent { get; set; }

    public string TaxCode { get; set; } = string.Empty;

    public DocumentAgroup? DocumentAgroup { get; set; }
}

