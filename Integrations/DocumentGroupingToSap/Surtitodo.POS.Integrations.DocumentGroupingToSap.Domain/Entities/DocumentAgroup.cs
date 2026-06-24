namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

/// <summary>
/// Documento agrupado pendiente de integración hacia SAP.
/// </summary>
public class DocumentAgroup
{
    public long Id { get; set; }

    public DateOnly AgroupDate { get; set; }

    public TimeOnly AgroupHour { get; set; }

    public DateTime AgroupDateTime { get; set; }

    public string DocumentAgroupType { get; set; } = string.Empty;

    public string WarehouseCode { get; set; } = string.Empty;

    public string CashBoxCode { get; set; } = string.Empty;

    public DateOnly DocDate { get; set; }

    public string CardCode { get; set; } = string.Empty;

    public string NumAtCard { get; set; } = string.Empty;

    public string IntegrationStatus { get; set; } = string.Empty;

    public DateOnly? IntegrationDate { get; set; }

    public TimeOnly? IntegrationHour { get; set; }

    public DateTime? IntegrationDateTime { get; set; }

    public int? IntegrationCode { get; set; }

    public string? IntegrationMessage { get; set; }

    public int? IntegrationHttpCode { get; set; }

    public string? IntegrationHttpMessage { get; set; }

    public string? IntegrationJsonRequestFile { get; set; }

    public string? IntegrationJsonResponseFile { get; set; }

    public long? DocNumSap { get; set; }

    public long? DocEntrySap { get; set; }

    public ICollection<DocumentAgroupLine> Lines { get; set; }
        = new List<DocumentAgroupLine>();
}