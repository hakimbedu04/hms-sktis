using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TpoFeeHdrPlanDto
    {
        public string TPOFeeCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string BrandGroupCode { get; set; }
        public string LocationCode { get; set; }
        public int? TPOFeeTempID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public int? StickPerBox { get; set; }
        public double? TPOPackageValue { get; set; }
        public double? TotalProdStick { get; set; }
        public double? TotalProdBox { get; set; }
        public double? TotalProdJKN { get; set; }
        public double? TotalProdJl1 { get; set; }
        public double? TotalProdJl2 { get; set; }
        public double? TotalProdJl3 { get; set; }
        public double? TotalProdJl4 { get; set; }
        public List<TPOFeeProductionDailyPlanDto> TpoFeeProductionDailyPlans { get; set; }
        public List<TPOFeeCalculationPlanDto> TpoFeeCalculationPlans { get; set; }
    }
}
