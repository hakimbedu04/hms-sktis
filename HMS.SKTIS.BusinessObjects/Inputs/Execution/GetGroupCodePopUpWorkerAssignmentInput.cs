using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetGroupCodePopUpWorkerAssignmentInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public int? Shift { get; set; }
        public string ProcessCode { get; set; }
        public DateTime Date { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }


    }
}
