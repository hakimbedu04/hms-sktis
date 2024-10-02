using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanTPOTPKTotalBoxDTO
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string TotalType { get; set; }
        public float? TargetSystem1 { get; set; }
        public float? TargetSystem2 { get; set; }
        public float? TargetSystem3 { get; set; }
        public float? TargetSystem4 { get; set; }
        public float? TargetSystem5 { get; set; }
        public float? TargetSystem6 { get; set; }
        public float? TargetSystem7 { get; set; }
        public float? TargetManual1 { get; set; }
        public float? TargetManual2 { get; set; }
        public float? TargetManual3 { get; set; }
        public float? TargetManual4 { get; set; }
        public float? TargetManual5 { get; set; }
        public float? TargetManual6 { get; set; }
        public float? TargetManual7 { get; set; }
        public float? TotalTargetSystem { get; set; }
        public float? TotalTargetManual { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public float? ProcessWorkHours1 { get; set; }
        public float? ProcessWorkHours2 { get; set; }
        public float? ProcessWorkHours3 { get; set; }
        public float? ProcessWorkHours4 { get; set; }
        public float? ProcessWorkHours5 { get; set; }
        public float? ProcessWorkHours6 { get; set; }
        public float? ProcessWorkHours7 { get; set; }
        public float? TotalWorkHours { get; set; }
    }
}
