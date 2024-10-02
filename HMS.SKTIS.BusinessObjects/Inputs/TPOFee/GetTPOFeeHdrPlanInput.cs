using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.TPOFee
{
    public class GetTPOFeeHdrPlanInput : BaseInput
    {
        public string LocationCode { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public string ParentLocationCode { get; set; }
    }
}
