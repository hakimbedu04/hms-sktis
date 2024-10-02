using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class TPOTPKCalculateDTO
    {
        public List<TPOTPKByProcessDTO> TPOTPKByProcess { get; set; }
        public List<PlanTPOTPKTotalBoxDTO> TPOTPTotals { get; set; }
            
    }
}
