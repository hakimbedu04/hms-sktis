using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanTargetProductionUnitDTO
    {
        public System.DateTime ProductionStartDate { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string TPUCode { get; set; }
        public int? WorkerRegister { get; set; }
        public int? WorkerAvailable { get; set; }
        public int? WorkerAlocation { get; set; }
        public int? PercentAttendance1 { get; set; }
        public float? PercentAttendance2 { get; set; }
        public float? PercentAttendance3 { get; set; }
        public float? PercentAttendance4 { get; set; }
        public float? PercentAttendance5 { get; set; }
        public float? PercentAttendance6 { get; set; }
        public float? PercentAttendance7 { get; set; }
        public float? HistoricalCapacityWorker1 { get; set; }
        public float? HistoricalCapacityWorker2 { get; set; }
        public float? HistoricalCapacityWorker3 { get; set; }
        public float? HistoricalCapacityWorker4 { get; set; }
        public float? HistoricalCapacityWorker5 { get; set; }
        public float? HistoricalCapacityWorker6 { get; set; }
        public float? HistoricalCapacityWorker7 { get; set; }
        public float? HistoricalCapacityGroup1 { get; set; }
        public float? HistoricalCapacityGroup2 { get; set; }
        public float? HistoricalCapacityGroup3 { get; set; }
        public float? HistoricalCapacityGroup4 { get; set; }
        public float? HistoricalCapacityGroup5 { get; set; }
        public float? HistoricalCapacityGroup6 { get; set; }
        public float? HistoricalCapacityGroup7 { get; set; }
        public float? TargetSystem1 { get; set; }
        public float? TargetSystem2 { get; set; }
        public float? TargetSystem3 { get; set; }
        public float? TargetSystem4 { get; set; }
        public float? TargetSystem5 { get; set; }
        public float? TargetSystem6 { get; set; }
        public float? TargetSystem7 { get; set; }
        public float? TargetManual1 { get; set; }
        public float? TargetManual2 { get; set; }
        public float? TargetManual3 { get; set; }
        public float? TargetManual4 { get; set; }
        public float? TargetManual5 { get; set; }
        public float? TargetManual6 { get; set; }
        public float? TargetManual7 { get; set; }
        public int? ProcessWorkHours1 { get; set; }
        public int? ProcessWorkHours2 { get; set; }
        public int? ProcessWorkHours3 { get; set; }
        public int? ProcessWorkHours4 { get; set; }
        public int? ProcessWorkHours5 { get; set; }
        public int? ProcessWorkHours6 { get; set; }
        public int? ProcessWorkHours7 { get; set; }
        public int? TotalWorkhours { get; set; }
        public float? TotalTargetSystem { get; set; }
        public float? TotalTargetManual { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
