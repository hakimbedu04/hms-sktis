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
    
    public partial class MstGenHoliday
    {
        public System.DateTime HolidayDate { get; set; }
        public string HolidayType { get; set; }
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public Nullable<bool> StatusActive { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstGenLocation MstGenLocation { get; set; }
    }
}
