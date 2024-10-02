namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class InventoryStockDTO
    {
        public int BeginningStock { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int EndingStock { get; set; }
    }
}
