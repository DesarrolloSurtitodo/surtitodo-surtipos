namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source
{
    public class DocumentsLines
    {
        public required string BOCODI { get; set; }
        public required string CACODI { get; set; }
        public required string TIPDOC { get; set; }
        public required string ARCODI { get; set; }
        public int TLQTT { get; set; }
        public decimal TLTOT { get; set; }
        public int TLPDTE { get; set; }
        public required string CODIGO_IVA { get; set; }
    }
}
