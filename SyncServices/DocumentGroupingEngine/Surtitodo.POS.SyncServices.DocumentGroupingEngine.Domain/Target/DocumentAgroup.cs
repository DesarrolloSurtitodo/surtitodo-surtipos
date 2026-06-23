namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target
{
    public class DocumentAgroup
    {
        public long Id { get; set; }
        public DateTime AgroupDate { get; set; }
        public TimeSpan AgroupHour { get; set; }
        public DateTime AgroupDateTime { get; set; }
        public string DocumentAgroupType { get; set; } = null!;
        public string WarehouseCode { get; set; } = null!;
        public string CashBoxCode { get; set; } = null!;
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; } = null!;
        public string NumAtCard { get; set; } = null!;
        public string IntegrationStatus { get; set; } = IntegrationStatusType.Pending;
        public DateTime? IntegrationDate { get; set; }
        public TimeSpan? IntegrationHour { get; set; }
        public DateTime? IntegrationDateTime { get; set; }
        public int? IntegrationCode { get; set; }
        public string? IntegrationMessage { get; set; }
        public int? IntegrationHttpCode { get; set; }
        public string? IntegrationHttpMessage { get; set; }
        public string? IntegrationJsonRequestFile { get; set; }
        public string? IntegrationJsonResponseFile { get; set; }
        public long? DocNumSap { get; set; }
        public long? DocEntrySap { get; set; }
        public List<DocumentAgroupLines> Lines { get; set; } = new();
    }
}
