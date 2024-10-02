using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningPlantIndividualCapacity
{
    public class PlanningPlantIndividualCapacityViewModel : ViewModelBase
    {
        public string BrandGroupCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public decimal? HoursCapacity3 { get; set; }
        public decimal? HoursCapacity5 { get; set; }
        public decimal? HoursCapacity6 { get; set; }
        public decimal? HoursCapacity7 { get; set; }
        public decimal? HoursCapacity8 { get; set; }
        public decimal? HoursCapacity9 { get; set; }
        public decimal? HoursCapacity10 { get; set; }
        public bool? StatusActive { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}