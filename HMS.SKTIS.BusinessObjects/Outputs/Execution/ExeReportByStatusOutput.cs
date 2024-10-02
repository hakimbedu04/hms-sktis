using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Outputs.Execution
{

    public class ExeReportByStatusActualWorkHourOutput
    {
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public double? ActualWorkHour { get; set; }
    }
}
