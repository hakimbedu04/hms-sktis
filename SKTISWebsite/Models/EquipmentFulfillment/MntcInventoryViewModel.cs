using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EquipmentFulfillment
{
    public class MntcInventoryViewModel
    {
        public string LocationCode { get; set; }
        public int? EndingStock { get; set; }
        public int? Quantity { get; set; }
    }
}