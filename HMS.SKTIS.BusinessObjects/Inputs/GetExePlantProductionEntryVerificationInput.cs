using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetExePlantProductionEntryVerificationInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DatePopUp { get; set; }
        public bool? ExeProductionEntryReleaseCannotNull { get; set; }
        public bool? UtilTransactionLogsCannotNull { get; set; }
    }
}
