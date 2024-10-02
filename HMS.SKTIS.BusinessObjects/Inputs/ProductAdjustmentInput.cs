using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class ProductAdjustmentInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string BrandCode { get; set; }
        public DateTime? ProductionDate { get; set; }

        public int KpsYear { get; set; }

        public int KpsWeek { get; set; }
    }
}
