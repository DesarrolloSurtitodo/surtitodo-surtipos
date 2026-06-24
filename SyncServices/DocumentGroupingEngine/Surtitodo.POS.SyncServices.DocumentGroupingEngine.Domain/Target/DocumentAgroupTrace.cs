namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target
{
    public class DocumentAgroupTrace
    {
        public long Id { get; set; }
        public long DocumentAgroupId { get; set; }
        public string BOCODI { get; set; } = null!;
        public string CACODI { get; set; } = null!;
        public string TIPDOC { get; set; } = null!;
        public int TICODI { get; set; }
        public decimal TITOT { get; set; }
        public DateTime TracedAt { get; set; }
        public DocumentAgroup DocumentAgroup { get; set; } = null!;
    }
}
