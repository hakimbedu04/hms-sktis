using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantWorkerAbsenteeismInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string Process { get; set; }
        public string GroupCode { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string EmployeeID { get; set; }
        public DateTime StartDateAbsent { get; set; }
        public DateTime EndDateAbsent { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string FilterType { get; set; }
        public int Year { get; set; }
        public int YearMonthFrom { get; set; }
        public int YearMonthTo { get; set; }
        public int YearWeekFrom { get; set; }
        public int YearWeekTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int WeekFrom { get; set; }
        public int WeekTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
