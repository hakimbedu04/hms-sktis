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
    
    public partial class ExePlantCKSubmit
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string CKNumber { get; set; }
        public Nullable<System.DateTime> CKDate { get; set; }
        public Nullable<int> GTValue { get; set; }
        public Nullable<int> BDSktis { get; set; }
        public Nullable<int> BDP1 { get; set; }
        public Nullable<int> Stock { get; set; }
        public Nullable<int> RowNumber { get; set; }
        public Nullable<decimal> Number { get; set; }
        public string CKRemark { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstGenBrand MstGenBrand { get; set; }
        public virtual MstPlantUnit MstPlantUnit { get; set; }
    }
}
