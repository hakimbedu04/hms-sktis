using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeReportDailyProductionInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string FilterLocationName { get; set; }
        public int YearFrom { get; set; }
        public int WeekFrom { get; set; }
    }
}
