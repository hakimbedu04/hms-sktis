using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstTPOProductionGroupInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string Process { get; set; }
        public string Status { get; set; }
    }
}
