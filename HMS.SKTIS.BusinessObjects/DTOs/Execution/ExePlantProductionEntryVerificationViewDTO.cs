using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantProductionEntryVerificationViewDTO
    {
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public int WorkHour { get; set; }
        public float? TPKValue { get; set; }
        public float? TotalTargetValue { get; set; }
        public float? TotalActualValue { get; set; }
        public decimal? TotalCapacityValue { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool? Flag_Manual { get; set; }
        public int? A { get; set; }
        public int? I { get; set; }
        public int? S { get; set; }
        public int? C { get; set; }
        public int? CH { get; set; }
        public int? CT { get; set; }
        public int? SLS_SLP { get; set; }
        public int? ETC { get; set; }
        public double? Plant { get; set; }
        public double? Actual { get; set; }
        public int VerifySystem { get; set; }
        public bool? VerifyManual { get; set; }
        public bool AlreadySubmit { get; set; }
        public bool DisableReturn { get; set; }
        public bool ProductionCardSubmit { get; set; }
        public bool CompletedActualWorkHours { get; set; }
        public bool ProductionEntryRelease { get; set; }
        public bool IsHadBeenReturned { get; set; }
        public bool IsHadBeenSubmitted { get; set; }

    }
}
