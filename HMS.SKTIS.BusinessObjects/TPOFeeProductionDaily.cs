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
    
    public partial class TPOFeeProductionDaily
    {
        public string TPOFeeCode { get; set; }
        public System.DateTime FeeDate { get; set; }
        public Nullable<int> KPSYear { get; set; }
        public Nullable<int> KPSWeek { get; set; }
        public Nullable<double> OuputSticks { get; set; }
        public Nullable<double> OutputBox { get; set; }
        public Nullable<double> JKN { get; set; }
        public Nullable<double> JL1 { get; set; }
        public Nullable<double> Jl2 { get; set; }
        public Nullable<double> Jl3 { get; set; }
        public Nullable<double> Jl4 { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<double> JKNJam { get; set; }
        public Nullable<double> JL1Jam { get; set; }
        public Nullable<double> JL2Jam { get; set; }
        public Nullable<double> JL3Jam { get; set; }
        public Nullable<double> JL4Jam { get; set; }
        public Nullable<double> JKNRp { get; set; }
        public Nullable<double> JL1Rp { get; set; }
        public Nullable<double> JL2Rp { get; set; }
        public Nullable<double> JL3Rp { get; set; }
        public Nullable<double> JL4Rp { get; set; }
        public Nullable<double> JKNBoxFinal { get; set; }
        public Nullable<double> JL1BoxFinal { get; set; }
        public Nullable<double> JL2BoxFinal { get; set; }
        public Nullable<double> JL3BoxFinal { get; set; }
        public Nullable<double> JL4BoxFinal { get; set; }
    
        public virtual TPOFeeHdr TPOFeeHdr { get; set; }
    }
}
