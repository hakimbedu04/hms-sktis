using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanTPUStatusDTO
    {
        public PlanTPUStatusDTO()
        {
            SubmitLog = null;
            Dates = null;
            Resubmit = false;
        }
        public UtilTransactionLogDTO SubmitLog { get; set; }
        public List<DateStateDTO> Dates { get; set; }
        public bool Resubmit { get; set; }
    }
}
