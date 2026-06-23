namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source
{
    public class Documents
    {
        public required string BOCODI {  get; set; } 
        public required string CACODI {  get; set; }
        public required string TIPDOC { get; set; }
        public int TICODI { get; set; }
        public DateTime TIDATA { get; set; }
        public required string CLCODI { get; set; }
        public decimal TITOT { get; set; }
    }
}
