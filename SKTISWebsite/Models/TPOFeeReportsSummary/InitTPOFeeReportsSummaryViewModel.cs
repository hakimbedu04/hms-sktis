using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SKTISWebsite.Models.TPOFeeReportsSummary
{
    public class InitTPOFeeReportsSummaryViewModel
    {
        public InitTPOFeeReportsSummaryViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }

        public IEnumerable<SelectListItem> YearSelectList { get; set; }

    }
}