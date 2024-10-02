using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MaintenanceReportEquipmentStock
{
    public class MaintenanceEquipmentStockReportViewModel
    {
        public long RowID { get; set; }
        public string InventoryDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? InTransit { get; set; }
        public int? QI { get; set; }
        public int? ReadyToUse { get; set; }
        public int? BadStock { get; set; }
        public int? TotalStockMntc { get; set; }
        public int? Used { get; set; }
        public int? Repair { get; set; }
        public int? TotalStockProd { get; set; }
    }
}