using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeCalculationPlanDto
    {
        public string TPOFeeCode { get; set; }
        public string ProductionFeeType { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
        public int? OrderFeeType { get; set; }
        public double? OutputProduction { get; set; }
        public double? OutputBiaya { get; set; }
        public double? Calculate { get; set; }
    }
}
