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
    
    public partial class PlanWeeklyProductionPlanning
    {
        public PlanWeeklyProductionPlanning()
        {
            this.PlanTargetProductionUnits = new HashSet<PlanTargetProductionUnit>();
        }
    
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public decimal Value1 { get; set; }
        public decimal Value2 { get; set; }
        public decimal Value3 { get; set; }
        public decimal Value4 { get; set; }
        public decimal Value5 { get; set; }
        public decimal Value6 { get; set; }
        public decimal Value7 { get; set; }
        public decimal Value8 { get; set; }
        public decimal Value9 { get; set; }
        public decimal Value10 { get; set; }
        public decimal Value11 { get; set; }
        public decimal Value12 { get; set; }
        public decimal Value13 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstGenBrand MstGenBrand { get; set; }
        public virtual MstGenLocation MstGenLocation { get; set; }
        public virtual ICollection<PlanTargetProductionUnit> PlanTargetProductionUnits { get; set; }
    }
}
