using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.TPOFeeReportsSummary
{
    public class TPOFeeReportsSummaryViewModel
    {

        public TPOFeeReportsSummaryViewModel()
        {
            ListWeekValue = new List<TPOFeeReportsSummaryWeeklyViewModel>();  
        }

        public string ParentLocation { get; set; }
        public bool IsParentRow { get; set; }
        public bool IsSummary { get; set; }

        public bool IsLocation { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationAbbr { get; set; }

        public int Year { get; set; }
        public int MaxWeek { get; set; }

        public string TotalCalculateString { get; set; }

        public List<TPOFeeReportsSummaryWeeklyViewModel> ListWeekValue { get; set; }
    }

    public class TPOFeeReportsSummaryWeeklyViewModel
    {
        public bool IsParentRow { get; set; }
        public int Week { get; set; }
        public string WeekValueString { get; set; }

        public string Location { get; set; }
    }
}