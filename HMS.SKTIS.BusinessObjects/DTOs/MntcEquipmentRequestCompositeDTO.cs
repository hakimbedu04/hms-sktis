using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MntcEquipmentRequestCompositeDTO
    {
        public System.DateTime RequestDate { get; set; }
        public string ItemCode { get; set; }
        public decimal Qty { get; set; }
        public string ItemDescription { get; set; }
        public int? ReadyToUse { get; set; }
        public int? OnUsed { get; set; }
        public int? OnRepair { get; set; }
        public decimal TotalQty { get; set; }
        public decimal? ApprovedQty { get; set; }
        public string LocationCode { get; set; }
        public string RequestNumber { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }

        public decimal QtyLeftOver { get; set; }
    }
}
