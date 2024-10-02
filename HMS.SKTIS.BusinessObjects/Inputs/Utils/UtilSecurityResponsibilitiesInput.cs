using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Utils
{
    public class UtilSecurityResponsibilitiesInput : BaseInput
    {
        public int? IDRole { get; set; }
        public string ResponsibilityName { get; set; }
        public int? IDResponsibility { get; set; }
    }
}
