using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetEblekReleaseApprovalInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string Process { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public DateTime Date { get; set; }
        public string Brand { get; set; }
        public string Group { get; set; }
    }
}
