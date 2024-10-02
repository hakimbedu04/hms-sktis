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
    
    public partial class PlanPlantIndividualCapacityReference
    {
        public string BrandGroupCode { get; set; }
        public string EmployeeID { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public Nullable<int> MinCapacity { get; set; }
        public Nullable<int> MaxCapacity { get; set; }
        public Nullable<int> AvgCapacity { get; set; }
        public Nullable<int> MedCapacity { get; set; }
        public Nullable<int> LatestCapacity { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstGenBrandGroup MstGenBrandGroup { get; set; }
        public virtual MstPlantUnit MstPlantUnit { get; set; }
        public virtual MstPlantEmpJobsDataAll MstPlantEmpJobsDataAll { get; set; }
    }
}