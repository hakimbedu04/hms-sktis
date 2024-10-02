using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanTPOTPKCompositeDTO
    {
        public int RowNumber { get; set; }
        public DateTime TPKTPOStartProductionDate { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string ProdGroup { get; set; }
        public string ProcessGroup { get; set; }
        public string LocationCode { get; set; }
        public string StatusEmp { get; set; }
        public string BrandCode { get; set; }
        public string TPKCode { get; set; }
        public int? WorkerRegister { get; set; }
        public int? WorkerAvailable { get; set; }
        public int? WorkerAlocation { get; set; }
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
        public float? ProcessWorkHours1 { get; set; }
        public float? ProcessWorkHours2 { get; set; }
        public float? ProcessWorkHours3 { get; set; }
        public float? ProcessWorkHours4 { get; set; }
        public float? ProcessWorkHours5 { get; set; }
        public float? ProcessWorkHours6 { get; set; }
        public float? ProcessWorkHours7 { get; set; }
        public float? TotalWorkhours { get; set; }
        public float? TotalTargetSystem { get; set; }
        public float? TotalTargetManual { get; set; }
        public int? TotalWorkHours1Prev3Weeks { get; set; }
        public int? TotalWorkHours2Prev3Weeks { get; set; }
        public int? TotalWorkHours3Prev3Weeks { get; set; }
        public int? TotalWorkHours4Prev3Weeks { get; set; }
        public int? TotalWorkHours5Prev3Weeks { get; set; }
        public int? TotalWorkHours6Prev3Weeks { get; set; }
        public int? TotalWorkHours7Prev3Weeks { get; set; }
        public int? TotalActualProductionPrev3Weeks { get; set; }
        public float? ProcessWorkHoursTemp { get; set; }
    }
}
