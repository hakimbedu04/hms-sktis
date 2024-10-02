using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeReportByGroupInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string ProcessGroup { get; set; }
        public string Brand { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? MonthFrom { get; set; }
        public int? MonthTo { get; set; }
        public int? WeekFrom { get; set; }
        public int? WeekTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string FilterType { get; set; }
        public int? YearFromMonth { get; set; }
        public int? YearFromWeek { get; set; }
        public int? YearToMonth { get; set; }
        public int? YearToWeek { get; set; }
    }
}
