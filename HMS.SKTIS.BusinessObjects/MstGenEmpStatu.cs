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
    
    public partial class MstGenEmpStatu
    {
        public MstGenEmpStatu()
        {
            this.ExeTPOActualWorkHours = new HashSet<ExeTPOActualWorkHour>();
            this.MstGenLocStatus = new HashSet<MstGenLocStatu>();
            this.ProductAdjustments = new HashSet<ProductAdjustment>();
        }
    
        public string StatusEmp { get; set; }
        public string StatusIdentifier { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<ExeTPOActualWorkHour> ExeTPOActualWorkHours { get; set; }
        public virtual ICollection<MstGenLocStatu> MstGenLocStatus { get; set; }
        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; }
    }
}
