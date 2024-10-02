using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantWorkerBalancingProcessPerGroupDTO
    {
        public string GroupCode { get; set; }
        public int Attend { get; set; }
        public int TPKAllocation { get; set; }
        public bool IsBalance { get; set; }
        public int AfterBalancingPerGroup { get; set; }
    }
}
