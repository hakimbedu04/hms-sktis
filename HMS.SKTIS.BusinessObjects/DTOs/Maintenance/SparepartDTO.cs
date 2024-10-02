namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class SparepartDTO
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string QtyConvert { get; set; }
        public string Quantity { get; set; }
        public bool IsFirstLoad { get; set; }
    }
}
