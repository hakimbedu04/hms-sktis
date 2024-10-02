using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExeReportDailyProductionAchievement
{
    public class ExeReportDailyProductionAchievementViewModel
    {
        public ExeReportDailyProductionAchievementViewModel()
        {
            DetailsData = new List<DataDailyProductionAchievment>();
        }
        public string SktBrandCode { get; set; }
        public List<DataDailyProductionAchievment> DetailsData { get; set; }
        public int CountData { get; set; }

        //Total/Day
        public string TotalMonday { get; set; }
        public string TotalTuesday { get; set; }
        public string TotalWednesday { get; set; }
        public string TotalThursday { get; set; }
        public string TotalFriday { get; set; }
        public string TotalSaturday { get; set; }
        public string TotalSunday { get; set; }

        //totalPerDay
        public string SumTotalAllDay { get; set; }
        public string SumAllTpkValue { get; set; }
        public string SumAllPackage { get; set; }
        public string SumAllVarianStick { get; set; }
        public string SumAllVarianPercen { get; set; }
        public string SumAllReliabilty { get; set; }

        //cumulativePercen
        public string CumulativeMonday { get; set; }
        public string CumulativeTuesday { get; set; }
        public string CumulativeWednesday { get; set; }
        public string CumulativeThursday { get; set; }
        public string CumulativeFriday { get; set; }
        public string CumulativeSaturday { get; set; }
        public string CumulativeSunday { get; set; }
        public string TotalAllCumulative { get; set; }
        
        //cumulativeStick
        public string StickCumulativeMonday { get; set; }
        public string StickCumulativeTuesday { get; set; }
        public string StickCumulativeWednesday { get; set; }
        public string StickCumulativeThursday { get; set; }
        public string StickCumulativeFriday { get; set; }
        public string StickCumulativeSaturday { get; set; }
        public string StickCumulativeSunday { get; set; }
        public string StickTotalAllCumulative { get; set; }

         //TotalHandRole
        public string TotalHandRoleMonday { get; set; }
        public string TotalHandRoleTuesday { get; set; }
        public string TotalHandrRoleWednesday { get; set; }
        public string TotalHandrRoleThursday { get; set; }
        public string TotalHandrRoleFriday { get; set; }
        public string TotalHandrRoleSaturday { get; set; }
        public string TotalHandrRoleSunday { get; set; }


    }


    public class DataDailyProductionAchievment
    {
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string ABBR { get; set; }
        public string ParentLocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string SKTBrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public string Production { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public string TPKValue { get; set; }
        public string WorkerCount { get; set; }
        public string Package { get; set; }
        public string StdStickPerHour { get; set; }

        public string ProductionMonday { get; set; }
        public string ProductionTuesday { get; set; }
        public string ProductionWednesday { get; set; }
        public string ProductionThursday { get; set; }
        public string ProductionFriday { get; set; }
        public string ProductionSaturday { get; set; }
        public string ProductionSunday { get; set; }
        public string TotalAllDay { get; set; }
        public string SumTpkValue { get; set; }
        public string VarianceStick { get; set; }
        public string VariancePersen { get; set; }
        public string Realiability { get; set; }

        
    }
}


