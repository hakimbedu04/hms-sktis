using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportDailyProductionAchievementViewDTO
    {
        public ExeReportDailyProductionAchievementViewDTO()
        {
            Details = new List<DataDailyProductionAchievmentDTO>();
        }
        public string SKTBrandCode { get; set; }
        public List<DataDailyProductionAchievmentDTO> Details { get; set; }
        public int CountData { get; set; }

        //Total/Day
        public int TotalMonday { get; set; }
        public int TotalTuesday { get; set; }
        public int TotalWednesday { get; set; }
        public int TotalThursday { get; set; }
        public int TotalFriday { get; set; }
        public int TotalSaturday { get; set; }
        public int TotalSunday { get; set; }

        //totalPerDay
        public int SumTotalAllDay { get; set; }
        public double? SumAllTpkValue { get; set; }
        public float SumAllPackage { get; set; }
        public double? SumAllVarianStick { get; set; }
        public double? SumAllVarianPercen { get; set; }
        public double? SumAllReliabilty { get; set; }

        //cumulativePercen
        public double? CumulativeMonday { get; set; }
        public double? CumulativeTuesday { get; set; }
        public double? CumulativeWednesday { get; set; }
        public double? CumulativeThursday { get; set; }
        public double? CumulativeFriday { get; set; }
        public double? CumulativeSaturday { get; set; }
        public double? CumulativeSunday { get; set; }
        public double? TotalAllCumulative { get; set; }

        //cumulativeStick
        public double? StickCumulativeMonday { get; set; }
        public double? StickCumulativeTuesday { get; set; }
        public double? StickCumulativeWednesday { get; set; }
        public double? StickCumulativeThursday { get; set; }
        public double? StickCumulativeFriday { get; set; }
        public double? StickCumulativeSaturday { get; set; }
        public double? StickCumulativeSunday { get; set; }
      
        //TotalHandRole
        public int TotalHandRoleMonday { get; set; }
        public int TotalHandRoleTuesday { get; set; }
        public int TotalHandrRoleWednesday { get; set; }
        public int TotalHandrRoleThursday { get; set; }
        public int TotalHandrRoleFriday { get; set; }
        public int TotalHandrRoleSaturday { get; set; }
        public int TotalHandrRoleSunday { get; set; }

    }

    public class DataDailyProductionAchievmentDTO
    {
        public string LocationCode { get; set; }
        public string ABBR { get; set; }
        public string BrandCode { get; set; }
        public string ParentLocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string SKTBrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public int? Production { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public double? TPKValue { get; set; }
        public int? WorkerCount { get; set; }
        public float Package { get; set; }
        public int? StdStickPerHour { get; set; }

        public int ProductionMonday { get; set; }
        public int ProductionTuesday { get; set; }
        public int ProductionWednesday { get; set; }
        public int ProductionThursday { get; set; }
        public int ProductionFriday { get; set; }
        public int ProductionSaturday { get; set; }
        public int ProductionSunday { get; set; }
        public int TotalAllDay { get; set; }
        public double? SumTpkValue { get; set; }
        public double? VarianceStick { get; set; }
        public double? VariancePersen { get; set; }
        public double? Realiability { get; set; }
        
       

    }
}
