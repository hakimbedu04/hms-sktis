using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanningReportSummaryProcessTargetsInput : BaseInput
    {
        public string Location { get; set; }
        public int Year { get; set; }
        public int Decimal { get; set; }
        public int Week { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string FilterType { get; set; }
    }
}
