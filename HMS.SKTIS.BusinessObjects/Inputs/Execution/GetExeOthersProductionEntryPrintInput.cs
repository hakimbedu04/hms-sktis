using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeOthersProductionEntryPrintInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string UnitCode { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public int? Shift { get; set; }
        public string Brand { get; set; }
        public string Process { get; set; }
        public string Remark { get; set; }
        public string GroupCode { get; set; }
    }
}
