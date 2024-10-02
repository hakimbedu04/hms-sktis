using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class MaintenanceExecutionInventoryViewInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string ItemType { get; set; }
        public DateTime? Date { get; set; }
    }
}
