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
    
    public partial class PlanTPOTargetProductionKelompokBox
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public Nullable<float> TargetSystem1 { get; set; }
        public Nullable<float> TargetSystem2 { get; set; }
        public Nullable<float> TargetSystem3 { get; set; }
        public Nullable<float> TargetSystem4 { get; set; }
        public Nullable<float> TargetSystem5 { get; set; }
        public Nullable<float> TargetSystem6 { get; set; }
        public Nullable<float> TargetSystem7 { get; set; }
        public Nullable<float> TargetManual1 { get; set; }
        public Nullable<float> TargetManual2 { get; set; }
        public Nullable<float> TargetManual3 { get; set; }
        public Nullable<float> TargetManual4 { get; set; }
        public Nullable<float> TargetManual5 { get; set; }
        public Nullable<float> TargetManual6 { get; set; }
        public Nullable<float> TargetManual7 { get; set; }
        public Nullable<float> TotalTargetSystem { get; set; }
        public Nullable<float> TotalTargetManual { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<float> ProcessWorkHours1 { get; set; }
        public Nullable<float> ProcessWorkHours2 { get; set; }
        public Nullable<float> ProcessWorkHours3 { get; set; }
        public Nullable<float> ProcessWorkHours4 { get; set; }
        public Nullable<float> ProcessWorkHours5 { get; set; }
        public Nullable<float> ProcessWorkHours6 { get; set; }
        public Nullable<float> ProcessWorkHours7 { get; set; }
        public Nullable<float> TotalWorkHours { get; set; }
    
        public virtual MstGenBrand MstGenBrand { get; set; }
        public virtual MstGenLocation MstGenLocation { get; set; }
    }
}
