using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EquipmentRepairPlant
{
    public class SparepartViewModel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        public int UOM { get; set; }
        public int CalculatedQuantity { get; set; }
        public int tempCalculatedQuantity { get; set; }
    }
}