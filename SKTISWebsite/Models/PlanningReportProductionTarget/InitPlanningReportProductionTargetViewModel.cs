using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.PlanningReportProductionTarget
{
    public class InitPlanningReportProductionTargetViewModel
    {
        public InitPlanningReportProductionTargetViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultMoth = DateTime.Now.Month;
        }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public int DefaultYear { get; set; }
        public int DefaultMoth { get; set; }
        public int? DefaultWeek { get; set; }
        public string TodayDate { get; set; }
        public List<DateTime> DateList { get; set; }
        public SelectList LocationSelectList { get; set; }
    }
}