using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MaintenanceInventoryDTO
    {
        public System.DateTime InventoryDate { get; set; }
        public string ItemStatus { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ItemType { get; set; }
        public string UOM { get; set; }
        public int? BeginningStock { get; set; }
        public int? StockIn { get; set; }
        public int? StockOut { get; set; }
        public int? EndingStock { get; set; }
        public string ItemDescription { get; set; }
    }
}
