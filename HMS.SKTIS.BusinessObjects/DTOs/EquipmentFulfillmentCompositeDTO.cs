using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class EquipmentFulfillmentCompositeDTO
    {
        public string LocationCode { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestNumber { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? FulFillmentDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? ReadyToUse { get; set; }
        public int? OnUse { get; set; }
        public int? OnRepair { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal? ApprovedQty { get; set; }
        public decimal? RequestToQty { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public string PurchaseNumber { get; set; }
        public int? Quantity { get; set; }
        public string LocationCodeForReqToLocation { get; set; }
    }
}
