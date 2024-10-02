using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.MaintenanceExecutionInventory
{
    public class MaintenanceExecutionInventoryViewModel
    {
        public DateTime InventoryDate { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemType { get; set; }
        public int StawIT { get; set; }
        public int InIT { get; set; }
        public int OutIT { get; set; }
        public int StackIT { get; set; }
        public int StawQI { get; set; }
        public int InQI { get; set; }
        public int OutQI { get; set; }
        public int StackQI { get; set; }
        public int StawReady { get; set; }
        public int InReady { get; set; }
        public int OutReady { get; set; }
        public int StackReady { get; set; }
        public int StawOU { get; set; }
        public int InOU { get; set; }
        public int OutOU { get; set; }
        public int StackOU { get; set; }
        public int StawOR { get; set; }
        public int InOR { get; set; }
        public int OutOR { get; set; }
        public int StackOR { get; set; }
        public int StawBS { get; set; }
        public int InBS { get; set; }
        public int OutBS { get; set; }
        public int StackBS { get; set; }
    }
}
