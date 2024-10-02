using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningPlantTPK
{
    public class PlantTPKTotalModel : ViewModelBase
    {
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
    }
}