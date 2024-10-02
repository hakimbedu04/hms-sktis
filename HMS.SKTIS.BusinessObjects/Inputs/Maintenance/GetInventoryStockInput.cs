using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetInventoryStockInput
    {
        public DateTime InventoryDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemStatus { get; set; }
        public string UnitCode { get; set; }
    }
}
