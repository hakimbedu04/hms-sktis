using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportDailyProductionAchievement
{
    public class ReportDailyProductionViewModel
    {
        public ReportDailyProductionViewModel()
        {
          Details = new List<ExeReportDailyProductionAchievementViewModel>(); 
        }
        public List<ExeReportDailyProductionAchievementViewModel> Details { get; set; }

        //TotalHandRole
        public string TotalHandRoleMonday { get; set; }
        public string TotalHandRoleTuesday { get; set; }
        public string TotalHandrRoleWednesday { get; set; }
        public string TotalHandrRoleThursday { get; set; }
        public string TotalHandrRoleFriday { get; set; }
        public string TotalHandrRoleSaturday { get; set; }
        public string TotalHandrRoleSunday { get; set; }
        public string SumAllTotalHandRole { get; set; }
        public string SumAllTotalHandRolePlanning { get; set; }
        public string SumAllTotalHandRoleVarianceStick { get; set; }
        public string SumAllTotalHandRoleVariancePercen { get; set; }
        public string SumAllTotalHandRolePackage { get; set; }
        public string SumAllTotalHandRoleReliability { get; set; }
        public string SumAllTotalHandRoleTheoriticalWhEqv { get; set; }

        public string TotalAllCumulativeStickMonday { get; set; }
        public string TotalAllCumulativeStickTuesday { get; set; }
        public string TotalAllCumulativeStickWednesday { get; set; }
        public string TotalAllCumulativeStickThursday { get; set; }
        public string TotalAllCumulativeStickFriday { get; set; }
        public string TotalAllCumulativeStickSaturday { get; set; }
        public string TotalAllCumulativeStickSunday { get; set; }

        public string TotalAllCumulativePercentMonday { get; set; }
        public string TotalAllCumulativePercentTuesday { get; set; }
        public string TotalAllCumulativePercentWednesday { get; set; }
        public string TotalAllCumulativePercentThursday { get; set; }
        public string TotalAllCumulativePercentFriday { get; set; }
        public string TotalAllCumulativePercentSaturday { get; set; }
        public string TotalAllCumulativePercentSunday { get; set; }
    }
}