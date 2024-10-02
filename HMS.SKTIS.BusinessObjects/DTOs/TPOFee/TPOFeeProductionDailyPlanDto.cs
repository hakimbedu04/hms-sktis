using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeProductionDailyPlanDto
    {
        public string TPOFeeCode { get; set; }
        public DateTime FeeDate { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
        public double? OuputSticks { get; set; }
        public double? OutputBox { get; set; }
        public double? JKN { get; set; }
        public double? JL1 { get; set; }
        public double? Jl2 { get; set; }
        public double? Jl3 { get; set; }
        public double? Jl4 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public double? JKNRp { get; set; }
        public double? JL1Rp { get; set; }
        public double? JL2Rp { get; set; }
        public double? JL3Rp { get; set; }
        public double? JL4Rp { get; set; }
        public double? JKNJam { get; set; }
        public double? JL1Jam { get; set; }
        public double? JL2Jam { get; set; }
        public double? JL3Jam { get; set; }
        public double? JL4Jam { get; set; }
    }
}
