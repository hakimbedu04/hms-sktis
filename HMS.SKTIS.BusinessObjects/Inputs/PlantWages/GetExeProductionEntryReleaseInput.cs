using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetExeProductionEntryReleaseInput : BaseInput
    {
        public string LocationCode { get; set; }
        public int? Unit { get; set; }
        public int? Shift { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
