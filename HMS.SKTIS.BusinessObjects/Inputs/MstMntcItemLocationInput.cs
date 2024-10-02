using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class MstMntcItemLocationInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
    }
}
