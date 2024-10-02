using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class MstGenBrandPackageItemInput : BaseInput
    {
        public string BrandGroupCode { get; set; }
        public string ItemCode { get; set; }
    }
}
