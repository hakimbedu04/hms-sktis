using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanningPlantIndividualCapacityByReferenceInput : BaseInput
    {
        public string Location { get; set; }
        public string Unit { get; set; }
        public string BrandGroupCode { get; set; }
        public string Process { get; set; }
        public string Group { get; set; }
        public int WorkHours { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }
}
