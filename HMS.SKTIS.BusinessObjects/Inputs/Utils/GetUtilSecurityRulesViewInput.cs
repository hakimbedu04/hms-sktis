using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Utils
{
    public class GetUtilSecurityRulesViewInput : BaseInput
    {
        public string Location { get; set; }
        public int IDRule { get; set; }
    }

}
