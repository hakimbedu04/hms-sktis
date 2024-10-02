using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanPlantAllocation
    {
        public string GroupCode { get; set; }
        public string ProcessSettingsCode { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string Shift { get; set; }
        public DateTime? PkPlantStartProdDate { get; set; }
    }
}
