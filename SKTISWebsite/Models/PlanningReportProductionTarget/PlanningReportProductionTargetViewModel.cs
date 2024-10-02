using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningReportProductionTarget
{
    public class PlanningReportProductionTargetViewModel : ViewModelBase
    {
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public double? TargetManual1 { get; set; }
        public double? TargetManual2 { get; set; }
        public double? TargetManual3 { get; set; }
        public double? TargetManual4 { get; set; }
        public double? TargetManual5 { get; set; }
        public double? TargetManual6 { get; set; }
        public double? TargetManual7 { get; set; }
    }
}