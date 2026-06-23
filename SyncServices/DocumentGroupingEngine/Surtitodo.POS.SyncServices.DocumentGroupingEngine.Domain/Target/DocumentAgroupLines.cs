namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target
{
    public class DocumentAgroupLines
    {
        public long Id { get; set; }
        public long DocumentAgroupId { get; set; }
        public string WarehouseCode { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public string TaxCode { get; set; } = null!;
        public DocumentAgroup DocumentAgroup { get; set; } = null!;
    }
}
