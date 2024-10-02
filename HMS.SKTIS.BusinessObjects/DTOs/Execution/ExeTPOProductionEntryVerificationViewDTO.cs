using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeTPOProductionEntryVerificationViewDTO
    {
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public string ProcessGroup { get; set; }
        public int Absent { get; set; }
        public double? TotalTPKValue { get; set; }
        public double? TotalActualValue { get; set; }
        public bool? VerifySystem { get; set; }
        public bool? VerifyManual { get; set; }
        public bool AlreadySubmit { get; set; }
        public string ProcessIdentifier { get; set; }
        public bool? Flag_Manual { get; set; }
        public bool IsRollingExist { get; set; }
        public bool IsPackingExist { get; set; }
        public bool IsWrappingExist { get; set; }
        public bool IsValidForSubmit { get; set; }

        // State -> INITIAL, SUBMITTED, CANCELSUBMIT
        public string State { get; set; }
    }
}
