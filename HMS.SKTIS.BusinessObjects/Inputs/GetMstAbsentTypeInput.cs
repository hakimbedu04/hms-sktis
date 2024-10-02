using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstAbsentTypeInput : BaseInput
    {
        public string AbsentType { get; set; }
        public bool? ActiveInAbsent { get; set; }
        public string LocationCode { get; set; }
        public bool? ActiveInEntry { get; set; }
    }
}
