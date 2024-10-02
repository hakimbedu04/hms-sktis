using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantMaterialUsagesInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string Unit { get; set; }
        public int? Shift { get; set; }
        public string Process { get; set; }
        public string BrandGroup { get; set; }
        public string Material { get; set; }
        public int? Year { get; set; }
        public int? Week { get; set; }
        public DateTime? Date { get; set; }
    }
}
