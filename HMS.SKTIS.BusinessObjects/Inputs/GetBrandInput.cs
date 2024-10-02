using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetBrandInput : BaseInput
    {
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

    }
}
