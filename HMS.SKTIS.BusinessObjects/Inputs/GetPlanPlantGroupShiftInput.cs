using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetPlanPlantGroupShiftInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProcessGroup { get; set; }
        public string Status { get; set; }


    }
}
