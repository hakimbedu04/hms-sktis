using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeProdStockInput : BaseInput
    {
        public string FilterLocation { get; set; }
        public string FilterLocationName { get; set; }
        public string FilterUnitCode { get; set; }
        public int? FilterYear { get; set; }
        public int? FilterYearMonthFrom { get; set; }
        public int? FilterMonthFrom { get; set; }
        public int? FilterYearMonthTo { get; set; }
        public int? FilterMonthTo { get; set; }
        public int? FilterYearWeekFrom { get; set; }
        public int? FilterWeekFrom { get; set; }
        public int? FilterYearWeekTo { get; set; }
        public int? FilterWeekTo { get; set; }
        public DateTime? FilterDateFrom { get; set; }
        public DateTime? FilterDateTo { get; set; }
        public bool IsFilterAnnualy { get; set; }
        public bool IsFilterMonthly { get; set; }
        public bool IsFilterWeekly { get; set; }
        public bool IsFilterDaily { get; set; }
    }
}
