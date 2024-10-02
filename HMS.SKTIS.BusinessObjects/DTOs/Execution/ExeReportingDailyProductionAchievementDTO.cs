using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class GetExeReportDailyProductionAchievementDTO
    {
        public string LocationCode { get; set; }
        public string SKTBrandCode { get; set; }
        public string BrandCode { get; set; }
        public string ABBR { get; set; }
        public string ParentLocationCode { get; set; }
        public Nullable<double> Monday { get; set; }
        public Nullable<double> Tuesday { get; set; }
        public Nullable<double> Wednesday { get; set; }
        public Nullable<double> Thursday { get; set; }
        public Nullable<double> Friday { get; set; }
        public Nullable<double> Saturday { get; set; }
        public Nullable<double> Sunday { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Planning { get; set; }
        public Nullable<double> VarianceStick { get; set; }
        public Nullable<decimal> VariancePercent { get; set; }
        public Nullable<decimal> ReliabilityPercent { get; set; }
        public Nullable<float> Package { get; set; }
        public Nullable<decimal> TWHEqv { get; set; }
    }
    public class ExeReportingDailyProductionAchievementDTOBrandCode
    {
        public string SKTBrandCode { get; set; }
        public string BrandCode { get; set; }
        public int CountBrandCode { get; set; }

        public IEnumerable<GetExeReportDailyProductionAchievementDTO> ListPerBrandCode { get; set; }

        public double? SubTotalMonday { get; set; }
        public double? SubTotalTuesday { get; set; }
        public double? SubTotalWednesday { get; set; }
        public double? SubTotalThursday { get; set; }
        public double? SubTotalFriday { get; set; }
        public double? SubTotalSaturday { get; set; }
        public double? SubTotalSunday { get; set; }
        public double? SubTotalTotal { get; set; }
        public double? SubTotalPlanning { get; set; }
        public double? SubTotalVarianceStick { get; set; }
        public decimal? SubTotalVariancePercent { get; set; }
        public decimal? SubTotalReliabilityPercent { get; set; }
        public float? SubTotalPackage { get; set; }
        public decimal? SubTotalTWHEqv { get; set; }
    }
    public class ExeReportingDailyProductionAchievementDTOSKTBrandCode
    {
        public string SKTBrandCode { get; set; }
        public int CountSKTBrandCode { get; set; }

        public List<ExeReportingDailyProductionAchievementDTOBrandCode> ListPerSKTBrandCode { get; set; }

        public double? TotalMonday { get; set; }
        public double? TotalTuesday { get; set; }
        public double? TotalWednesday { get; set; }
        public double? TotalThursday { get; set; }
        public double? TotalFriday { get; set; }
        public double? TotalSaturday { get; set; }
        public double? TotalSunday { get; set; }
        public double? TotalTotal { get; set; }
        public double? TotalPlanning { get; set; }
        public double? TotalVarianceStick { get; set; }
        public decimal? TotalVariancePercent { get; set; }
        public decimal? TotalReliabilityPercent { get; set; }
        public float? TotalPackage { get; set; }
        public decimal? TotalTWHEqv { get; set; }

        public double? CumulativeTotalMonday { get; set; }
        public double? CumulativeTotalTuesday { get; set; }
        public double? CumulativeTotalWednesday { get; set; }
        public double? CumulativeTotalThursday { get; set; }
        public double? CumulativeTotalFriday { get; set; }
        public double? CumulativeTotalSaturday { get; set; }
        public double? CumulativeTotalSunday { get; set; }

        public double? CumulativePercentTotalMonday { get; set; }
        public double? CumulativePercentTotalTuesday { get; set; }
        public double? CumulativePercentTotalWednesday { get; set; }
        public double? CumulativePercentTotalThursday { get; set; }
        public double? CumulativePercentTotalFriday { get; set; }
        public double? CumulativePercentTotalSaturday { get; set; }
        public double? CumulativePercentTotalSunday { get; set; }
    }
}
