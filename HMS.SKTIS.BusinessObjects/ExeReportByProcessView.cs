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
    
    public partial class ExeReportByProcessView
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public int Shift { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int UOMOrder { get; set; }
        public double Production { get; set; }
        public double KeluarBersih { get; set; }
        public double RejectSample { get; set; }
        public double BeginningStock { get; set; }
        public double EndingStock { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string BrandGroupCode { get; set; }
    }
}
