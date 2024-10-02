using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using SKTISWebsite.Models.EquipmentRequest;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.LookupList;


namespace SKTISWebsite.Models.PlanningReportSummaryProcessTargets
{
    public class InitPlanningReportSummaryProcessTargetsViewModel
    {
        public InitPlanningReportSummaryProcessTargetsViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList LocationSelectList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public DateTime? DefaultDateFrom { get; set; }
        public DateTime? DefaultDateTo { get; set; }

        public List<MasterGenLocationDesc> LocationDescs { get; set; }

    }
}