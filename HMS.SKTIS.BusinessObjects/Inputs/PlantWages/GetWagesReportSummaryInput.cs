using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetWagesReportSummaryInput : BaseInput
    {
        public DateTime DateTo { get; set; }
        public DateTime DateFrom { get; set; }
        public int Week { get; set; }
        public int WeekTo { get; set; }
        public int Year { get; set; }
        public int YearTo { get; set; }
        public string BrandGroupCode { get; set; }
        
        public string FilterType { get; set; }
    }
}
