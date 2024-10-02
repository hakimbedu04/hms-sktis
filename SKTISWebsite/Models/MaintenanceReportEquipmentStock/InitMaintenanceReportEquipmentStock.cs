using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MaintenanceReportEquipmentStock
{
    public class InitMaintenanceReportEquipmentStock
    {
        public InitMaintenanceReportEquipmentStock()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int? DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultUnitCode { get; set; }
        public DateTime DefaultDate { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }

        public List<LocationLookupList> LocationLookupList { get; set; }
    }
}