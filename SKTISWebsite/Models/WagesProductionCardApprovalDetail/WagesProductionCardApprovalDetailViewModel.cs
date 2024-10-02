using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesProductionCardApprovalDetail
{
    public class WagesProductionCardApprovalDetailViewModel
    {
        public string ProductionCardCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public DateTime ProductionDate { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public int? Worker { get; set; }
        public float? Production { get; set; }
        public float? UpahLain { get; set; }
        public int? A { get; set; }
        public int? C { get; set; }
        public int? CH { get; set; }
        public int? CT { get; set; }
        public int? I { get; set; }
        public int? LL { get; set; }
        public int? LO { get; set; }
        public int? LP { get; set; }
        public int? MO { get; set; }
        public int? PG { get; set; }
        public int? S { get; set; }
        public int? SB { get; set; }
        public int? SKR { get; set; }
        public int? SL4 { get; set; }
        public int? SLP { get; set; }
        public int? SLS { get; set; }
        public int? T { get; set; }
        public int? TL { get; set; }
        public bool Selected { get; set; }
    }
}