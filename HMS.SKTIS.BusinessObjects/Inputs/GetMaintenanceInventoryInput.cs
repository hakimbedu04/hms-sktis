using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMaintenanceInventoryInput : BaseInput
    {
        public DateTime? InventoryDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ItemStatus { get; set; }
    }
}
