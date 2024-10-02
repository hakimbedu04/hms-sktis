using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeTPOProductionEntryVerificationDTO
    {
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public float WorkHour { get; set; }
        public float? TotalTPKValue { get; set; }
        public float? TotalActualValue { get; set; }
        public bool? VerifySystem { get; set; }
        public bool? VerifyManual { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool AlreadySave { get; set; }
        public bool Flag_Manual { get; set; }
    }
}
