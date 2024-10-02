using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlantWIPStockInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
    }
}
