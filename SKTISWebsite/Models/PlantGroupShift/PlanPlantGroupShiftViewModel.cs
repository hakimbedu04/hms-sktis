using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlantGroupShift
{
    public class PlanPlantGroupShiftViewModel : ViewModelBase
    {
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessGroup { get; set; }
        public string LocationCode { get; set; }
        public int Shift { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}