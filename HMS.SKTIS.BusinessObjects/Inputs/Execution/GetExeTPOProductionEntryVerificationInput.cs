using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeTPOProductionEntryVerificationInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public DateTime? ProductionDate { get; set; }
        //public List<string> Val { get; set; }
        public string Check { get; set; }
    }
}
