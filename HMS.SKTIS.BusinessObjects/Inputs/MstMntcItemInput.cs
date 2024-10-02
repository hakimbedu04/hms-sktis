using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class MstMntcItemInput : BaseInput
    {
        public string ItemCode { get; set; }
        public string ItemType { get; set; }
    }
}
