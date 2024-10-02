using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.TPOFee
{
    public class GetTPOFeeExeActualInput : BaseInput
    {
        public string Regional { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public bool ApprovalPage { get; set; }
        public int DestinationRole { get; set; }
    }
}
