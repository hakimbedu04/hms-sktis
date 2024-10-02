using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EquipmentFulfillment
{
    public class ItemDetailViewModel
    {
        public string ItemDescription { get; set; }
        public int? ReadyToUse { get; set; }
        public int? OnUse { get; set; }
        public int? OnRepair { get; set; }
        public decimal RequestedQuantity { get; set; }
    }
}