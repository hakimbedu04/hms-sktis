//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HMS.SKTIS.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    
    public partial class PlanPlantIndividualCapacityByReferenceView
    {
        public System.DateTime ProductionDate { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public string BrandCode { get; set; }
        public int WorkHour { get; set; }
        public float MinimumValue { get; set; }
        public float MaximumValue { get; set; }
        public double AverageValue { get; set; }
        public float MedianValue { get; set; }
        public float LatestValue { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public Nullable<decimal> HoursCapacity3 { get; set; }
        public Nullable<decimal> HoursCapacity5 { get; set; }
        public Nullable<decimal> HoursCapacity6 { get; set; }
        public Nullable<decimal> HoursCapacity7 { get; set; }
        public Nullable<decimal> HoursCapacity8 { get; set; }
        public Nullable<decimal> HoursCapacity9 { get; set; }
        public Nullable<decimal> HoursCapacity10 { get; set; }
    }
}