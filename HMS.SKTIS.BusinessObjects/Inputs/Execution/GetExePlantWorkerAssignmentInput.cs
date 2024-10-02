using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantWorkerAssignmentInput : BaseInput
    {
        public string LocationCode { get; set; }

        public string LocationCompat { get; set; }
        public string UnitCode { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime? Date { get; set; }
        public int Shift { get; set; }
        public DateTime? ProductionDateFrom { get; set; }
        public DateTime? ProductionDateTo { get; set; }
        public string SourceBrandCode { get; set; }
        public string DateTypeFilter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime OldStartDate { get; set; }
        public DateTime OldEndDate { get; set; }
        public string LocationSource { get; set; }
        public string UnitSource { get; set; }
        public string BrandSource { get; set; }
        public string GroupCodeSource { get; set; }
        public string LocationDest { get; set; }
        public string UnitDest { get; set; }
        public string BrandDest { get; set; }
        public string GroupCodeDest { get; set; }
    }
}
