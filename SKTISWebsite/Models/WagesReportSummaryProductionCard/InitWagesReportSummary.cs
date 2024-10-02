using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.WagesReportSummaryProductionCard
{
    public class InitWagesReportSummary
    {
        public InitWagesReportSummary()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        
    }
}