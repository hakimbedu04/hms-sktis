using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportDailyProductionAchievement
{
    public class ExeReportDailyProdAchievementFinalViewModel
    {
        public List<ExeReportingDailyProductionAchievementSKTBrandCodeViewModel> ListPerSKTBrandCode { get; set; }
        public string TotalMondayHandRolled { get; set; }
        public string TotalTuesdayHandRolled { get; set; }
        public string TotalWednesdayHandRolled { get; set; }
        public string TotalThursdayHandRolled { get; set; }
        public string TotalFridayHandRolled { get; set; }
        public string TotalSaturdayHandRolled { get; set; }
        public string TotalSundayHandRolled { get; set; }
        public string TotalTotalHandRolled { get; set; }
        public string TotalPlanningHandRolled { get; set; }
        public string TotalVarianceStickHandRolled { get; set; }
        public string TotalVariancePercentHandRolled { get; set; }
        public string TotalReliabilityPercentHandRolled { get; set; }
        public string TotalPackageHandRolled { get; set; }
        public string TotalTWHEqvHandRolled { get; set; }

        public string TotalAllCumulativeStickMonday { get; set; }
        public string TotalAllCumulativeStickTuesday { get; set; }
        public string TotalAllCumulativeStickWednesday { get; set; }
        public string TotalAllCumulativeStickThursday{ get; set; }
        public string TotalAllCumulativeStickFriday { get; set; }
        public string TotalAllCumulativeStickSaturday { get; set; }
        public string TotalAllCumulativeStickSunday { get; set; }

        public string TotalAllCumulativePercentMonday { get; set; }
        public string TotalAllCumulativePercentTuesday { get; set; }
        public string TotalAllCumulativePercentWednesday { get; set; }
        public string TotalAllCumulativePercentThursday { get; set; }
        public string TotalAllCumulativePercentFriday { get; set; }
        public string TotalAllCumulativePercentSaturday{ get; set; }
        public string TotalAllCumulativePercentSunday { get; set; }
    }

    public class GetExeReportDailyProductionAchievementViewModel
    {
        public string LocationCode { get; set; }
        public string SKTBrandCode { get; set; }
        public string BrandCode { get; set; }
        public string ABBR { get; set; }
        public string ParentLocationCode { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Total { get; set; }
        public string Planning { get; set; }
        public string VarianceStick { get; set; }
        public string VariancePercent { get; set; }
        public string ReliabilityPercent { get; set; }
        public string Package { get; set; }
        public string TWHEqv { get; set; }
    }
    public class ExeReportingDailyProductionAchievementBrandCodeViewModel
    {
        public string SKTBrandCode { get; set; }
        public string BrandCode { get; set; }
        public int CountBrandCode { get; set; }

        public IEnumerable<GetExeReportDailyProductionAchievementViewModel> ListPerBrandCode { get; set; }

        public string SubTotalMonday { get; set; }
        public string SubTotalTuesday { get; set; }
        public string SubTotalWednesday { get; set; }
        public string SubTotalThursday { get; set; }
        public string SubTotalFriday { get; set; }
        public string SubTotalSaturday { get; set; }
        public string SubTotalSunday { get; set; }
        public string SubTotalTotal { get; set; }
        public string SubTotalPlanning { get; set; }
        public string SubTotalVarianceStick { get; set; }
        public string SubTotalVariancePercent { get; set; }
        public string SubTotalReliabilityPercent { get; set; }
        public string SubTotalPackage { get; set; }
        public string SubTotalTWHEqv { get; set; }
    }
    public class ExeReportingDailyProductionAchievementSKTBrandCodeViewModel
    {
        public string SKTBrandCode { get; set; }
        public int CountSKTBrandCode { get; set; }

        public List<ExeReportingDailyProductionAchievementBrandCodeViewModel> ListPerSKTBrandCode { get; set; }

        public string TotalMonday { get; set; }
        public string TotalTuesday { get; set; }
        public string TotalWednesday { get; set; }
        public string TotalThursday { get; set; }
        public string TotalFriday { get; set; }
        public string TotalSaturday { get; set; }
        public string TotalSunday { get; set; }
        public string TotalTotal { get; set; }
        public string TotalPlanning { get; set; }
        public string TotalVarianceStick { get; set; }
        public string TotalVariancePercent { get; set; }
        public string TotalReliabilityPercent { get; set; }
        public string TotalPackage { get; set; }
        public string TotalTWHEqv { get; set; }

        public string CumulativeTotalMonday { get; set; }
        public string CumulativeTotalTuesday { get; set; }
        public string CumulativeTotalWednesday { get; set; }
        public string CumulativeTotalThursday { get; set; }
        public string CumulativeTotalFriday { get; set; }
        public string CumulativeTotalSaturday { get; set; }
        public string CumulativeTotalSunday { get; set; }
               
        public string CumulativePercentTotalMonday { get; set; }
        public string CumulativePercentTotalTuesday { get; set; }
        public string CumulativePercentTotalWednesday { get; set; }
        public string CumulativePercentTotalThursday { get; set; }
        public string CumulativePercentTotalFriday { get; set; }
        public string CumulativePercentTotalSaturday { get; set; }
        public string CumulativePercentTotalSunday { get; set; }
    }
}