using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningPlantIndividualCapacity
{
    public class PlanningPlantIndividualCapacityByReferenceViewModel : ViewModelBase
    {
        public int? MinimumValue { get; set; }
        public int? MaximumValue { get; set; }
        public int? AverageValue { get; set; }
        public double? MedianValue { get; set; }
        public int? LatestValue { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public decimal? HoursCapacity { get; set; }
    }
}