using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EquipmentFulfillment
{
    public class EquipmentFulfillmentViewModel : ViewModelBase
    {
        public string FulfillmentDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? ReadyToUse { get; set; }
        public int? OnUse { get; set; }
        public int? OnRepair { get; set; }
        public decimal RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal? RequestToOthersQuantity { get; set; }
        public decimal? PurchaseQty { get; set; }
        public string PurchaseNumber { get; set; }
        public int? Quantity { get; set; } //for Update MntcRequestToLocation (QtyFromLocation)

        public List<MntcInventoryViewModel> RequestToOthersQuantityDetails { get; set; } 
        

        
        
    }
}