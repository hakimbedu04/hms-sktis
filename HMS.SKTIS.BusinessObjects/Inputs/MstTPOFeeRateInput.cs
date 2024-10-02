using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class MstTPOFeeRateInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string Year { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartDate { get; set; }
    }
}
