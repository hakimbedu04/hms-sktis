using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanningPlantIndividualCapacityWorkHourInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string Unit { get; set; }
        public string BrandCode { get; set; }
        public string Process { get; set; }
        public string Group { get; set; }
        public string CapacityOfProcess { get; set; }
        public string Date { get; set; }
    }
}
