using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetItemDisposalInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string FilterType { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int? WeekFrom { get; set; }
        public int? WeekTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? date { get; set; }

        public int Year { get; set; }
        public int YearMonthFrom { get; set; }
        public int YearMonthTo { get; set; }
        public int YearWeekFrom { get; set; }
        public int YearWeekTo { get; set; }

    }
}
