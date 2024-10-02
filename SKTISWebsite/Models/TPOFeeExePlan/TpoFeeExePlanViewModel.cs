using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.TPOFeeExePlan
{
    public class TpoFeeExePlanViewModel
    {
        public bool Checkbox { get; set; }
        public string TpoFeeCode { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string SktBrandCode { get; set; }
        public double? JknBox { get; set; }
        public double? Jl1Box { get; set; }
        public double? Jl2Box { get; set; }
        public double? Jl3Box { get; set; }
        public double? Jl4Box { get; set; }
        public double? ProductionCost { get; set; }
        public double? MaklonFee { get; set; }
        public double? ProductivityIncentives { get; set; }
        public double? MaklonFeeTwoPercent { get; set; }
        public double? ProductivityIncentivesTwoPercent { get; set; }
        public double? ProductionCostTenPercent { get; set; }
        public double? MaklonFeeTenPercent { get; set; }
        public double? ProductivityIncentivesTenPercent { get; set; }
        public double? TotalCost { get; set; }
    }
}