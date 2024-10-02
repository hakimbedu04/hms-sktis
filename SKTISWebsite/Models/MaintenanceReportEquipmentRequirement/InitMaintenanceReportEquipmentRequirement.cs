using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MaintenanceReportEquipmentRequirement
{
    public class InitMaintenanceReportEquipmentRequirement
    {
        public List<LocationLookupList> RegionalChildLocation { get; set; }
        public List<LocationLookupList> PLNTAndRegionalChildLocation { get; set; }
    }
}