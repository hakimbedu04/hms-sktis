using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstGenProcessesInput : BaseInput
    {
        public string ProcessName { get; set; }
        public bool WIP { get; set; }
        public bool StatusActive { get; set; }
    }
}
