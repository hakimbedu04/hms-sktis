using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantProductionEntryVerificationBrandInput : BaseInput
    {
        public string LocationCode { get; set; }
        public DateTime? Date { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }

    }
}
