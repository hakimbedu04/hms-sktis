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
    
    public partial class ExeReportEMSSubmitted
    {
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<double> UplProduceQty { get; set; }
        public Nullable<double> UplPackedQty { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstGenBrand MstGenBrand { get; set; }
        public virtual MstGenLocation MstGenLocation { get; set; }
    }
}