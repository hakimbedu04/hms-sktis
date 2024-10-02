using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanTmpWeeklyProductionPlanning
{
    public class PlanTmpWeeklyProductionPlanningViewModel
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public float Value3 { get; set; }
        public float Value4 { get; set; }
        public float Value5 { get; set; }
        public float Value6 { get; set; }
        public float Value7 { get; set; }
        public float Value8 { get; set; }
        public float Value9 { get; set; }
        public float Value10 { get; set; }
        public float Value11 { get; set; }
        public float Value12 { get; set; }
        public float Value13 { get; set; }
        public bool? IsValid { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}