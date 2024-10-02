using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetMaintenanceEquipmentStockReportInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public DateTime? InventoryDate { get; set; }
        public DateTime? Date { get; set; }
    }
}
