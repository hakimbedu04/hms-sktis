using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantWorkerBalancingProcessPerUnitDTO
    {
        public string Process { get; set; }
        public int Attend { get; set; }
        public int Recommendation { get; set; }
        public int AfterBalancing { get; set; }
        public int AttendAllProcess { get; set; }
        public List<ExePlantWorkerBalancingProcessPerGroupDTO> ListBalancingProcessPerGroup { get; set; }
    }
}
