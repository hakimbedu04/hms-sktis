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
    
    public partial class TPOFeeReportsProductionWeeklyView
    {
        public string Regional { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public Nullable<decimal> UMK { get; set; }
        public string BrandGroupCode { get; set; }
        public float Package { get; set; }
        public Nullable<double> JKN { get; set; }
        public Nullable<double> JL1 { get; set; }
        public Nullable<double> JL2 { get; set; }
        public Nullable<double> JL3 { get; set; }
        public Nullable<double> JL4 { get; set; }
        public Nullable<double> JasaManajemen { get; set; }
        public Nullable<decimal> ProductivityIncentives { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string MemoRef { get; set; }
        public Nullable<double> TotalProdJKN { get; set; }
        public Nullable<double> TotalProdJl1 { get; set; }
        public Nullable<double> TotalProdJl2 { get; set; }
        public Nullable<double> TotalProdJl3 { get; set; }
        public Nullable<double> TotalProdJl4 { get; set; }
    }
}
