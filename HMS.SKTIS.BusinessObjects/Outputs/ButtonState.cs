using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace HMS.SKTIS.BusinessObjects.Outputs
{
    public class ButtonState
    {
        public bool Save { get; set; }
        public bool Submit { get; set; }
        public bool CancelSubmit { get; set; }
        public bool Approve { get; set; }
        public bool Return { get; set; }
        public bool SendApproval { get; set; }
        public bool Complete { get; set; }

    }
}
