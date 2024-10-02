using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanningReportProductionTargetInput : BaseInput
    {
        public int Year { get; set; }
        public int Decimal { get; set; }
        public int Week { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
    }
}
