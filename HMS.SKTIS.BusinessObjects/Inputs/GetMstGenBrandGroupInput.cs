using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstGenBrandGroupInput : BaseInput
    {
        public string BrandFamily { get; set; }
        public string BrandGroupCode { get; set; }
        public string PackType { get; set; }
        public string ClassType { get; set; }
    }
}
