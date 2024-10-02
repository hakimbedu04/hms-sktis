using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetWagesReportAbsentViewInput : BaseInput
    {
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string Process { get; set; }
        public string ProdGroup { get; set; }
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
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }
    }
}
