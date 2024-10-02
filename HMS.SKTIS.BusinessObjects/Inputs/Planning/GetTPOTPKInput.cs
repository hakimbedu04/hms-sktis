using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetTPOTPKInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string TPKCode { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }

        public Boolean FromPopup { get; set; }

        public string[] ExpelledGroup { get; set; }
    }
}
