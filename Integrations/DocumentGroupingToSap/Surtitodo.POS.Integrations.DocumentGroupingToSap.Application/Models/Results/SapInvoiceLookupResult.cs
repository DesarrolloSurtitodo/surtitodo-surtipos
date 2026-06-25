namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;

public sealed class SapInvoiceLookupResult { 
    public long DocEntry { get; init; } 
    public long DocNum { get; init; }
    public string DocType { get; set; } = string.Empty; // "OINV" | "ORIN"
}
