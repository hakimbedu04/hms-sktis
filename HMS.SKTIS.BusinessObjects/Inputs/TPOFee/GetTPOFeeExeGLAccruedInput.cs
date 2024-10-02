using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.TPOFee
{
    public class GetTPOFeeExeGLAccruedInput : BaseInput
    {
        public GetTPOFeeExeGLAccruedInput()
        {
            SubmitMessage = null;
        }

        public string param1 { get; set; }
        public string Regional { get; set; }
        public int KpsYear { get; set; }
        public int KpsWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string LocationName { get; set; }
        public int StickPerBox { get; set; }
        public string SubmitMessage { get; set; }
    }
}
