using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetBrandGroupMaterialInput : BaseInput
    {
        public GetBrandGroupMaterialInput()
        {
            ProcessGroup = null;
        }
        //public string Location { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
    }
}
