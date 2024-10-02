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
    
    public partial class ExeTPOProductionEntryVerification
    {
        public ExeTPOProductionEntryVerification()
        {
            this.ExeTPOProductions = new HashSet<ExeTPOProduction>();
        }
    
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public float WorkHour { get; set; }
        public Nullable<float> TotalTPKValue { get; set; }
        public Nullable<float> TotalActualValue { get; set; }
        public Nullable<bool> VerifySystem { get; set; }
        public Nullable<bool> VerifyManual { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> Flag_Manual { get; set; }
    
        public virtual ICollection<ExeTPOProduction> ExeTPOProductions { get; set; }
    }
}
