using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanningPlantIndividualCapacityByReferenceDTO
    {
        public System.DateTime ProductionDate { get; set; }
        public DateTime ProductionEntryDate { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public string BrandCode { get; set; }
        public int WorkHours { get; set; }
        public int? MinimumValue { get; set; }
        public int? MaximumValue { get; set; }
        public int? AverageValue { get; set; }
        public int? MedianValue { get; set; }
        public int? LatestValue { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public decimal? HoursCapacity3 { get; set; }
        public decimal? HoursCapacity5 { get; set; }
        public decimal? HoursCapacity6 { get; set; }
        public decimal? HoursCapacity7 { get; set; }
        public decimal? HoursCapacity8 { get; set; }
        public decimal? HoursCapacity9 { get; set; }
        public decimal? HoursCapacity10 { get; set; }
    }
}
