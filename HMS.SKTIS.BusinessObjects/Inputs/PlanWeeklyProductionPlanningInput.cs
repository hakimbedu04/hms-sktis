using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class PlanWeeklyProductionPlanningInput : BaseInput
    {
        public string BrandFamily { get; set; }
        public string BrandGroupCodes { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }

    }
}
