using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MaintenanceReportEquipmentRequirement
{
    public class EquipmentRequirementItemViewModel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public List<int> Quantity { get; set; }
    }
}