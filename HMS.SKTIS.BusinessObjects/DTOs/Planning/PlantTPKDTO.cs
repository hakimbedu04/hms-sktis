using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlantTPKDTO
    {
        public DateTime TPKPlantStartProductionDate { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int Shift { get; set; }
        public int? WorkerRegister { get; set; }
        public int? WorkerAvailable { get; set; }
        public int? WorkerAllocation { get; set; }
        public int? WIP1 { get; set; }
        public int? WIP2 { get; set; }
        public int? WIP3 { get; set; }
        public int? WIP4 { get; set; }
        public int? WIP5 { get; set; }
        public int? WIP6 { get; set; }
        public int? WIP7 { get; set; }
        public float? PercentAttendance1 { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public float? DailyWHWeightedResult1 { get; set; }
        public float? DailyWHWeightedResult2 { get; set; }
        public float? DailyWHWeightedResult3 { get; set; }
        public float? DailyWHWeightedResult4 { get; set; }
        public float? DailyWHWeightedResult5 { get; set; }
        public float? DailyWHWeightedResult6 { get; set; }
        public float? DailyWHWeightedResult7 { get; set; }
        public decimal? Target1
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker1) * Convert.ToDecimal(PercentAttendance1) * Convert.ToDecimal(DailyWHWeightedResult1)), 6);
            }
        }
        public decimal? Target2
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker2) * Convert.ToDecimal(PercentAttendance2) * Convert.ToDecimal(DailyWHWeightedResult2)), 6);
            }
        }
        public decimal? Target3
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker3) * Convert.ToDecimal(PercentAttendance3) * Convert.ToDecimal(DailyWHWeightedResult3)), 6);
            }
        }
        public decimal? Target4
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker4) * Convert.ToDecimal(PercentAttendance4) * Convert.ToDecimal(DailyWHWeightedResult4)), 6);
            }
        }
        public decimal? Target5
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker5) * Convert.ToDecimal(PercentAttendance5) * Convert.ToDecimal(DailyWHWeightedResult5)), 6);
            }
        }
        public decimal? Target6
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker6) * Convert.ToDecimal(PercentAttendance6) * Convert.ToDecimal(DailyWHWeightedResult6)), 6);
            }
        }
        public decimal? Target7
        {
            get
            {
                return Math.Round((Convert.ToDecimal(WorkerAllocation) * Convert.ToDecimal(HistoricalCapacityWorker7) * Convert.ToDecimal(PercentAttendance7) * Convert.ToDecimal(DailyWHWeightedResult7)), 6);
            }
        }
    }
}
