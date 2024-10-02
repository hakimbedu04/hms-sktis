using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ExeTPOProductionEntry
{
    public class ExeTPOProductionEntryViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string StatusEmp { get; set; }
        public int StatusIdentifier { get; set; }
        public string ProductionGroup { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string ProductionDate { get; set; }
        public string ProductionEntryCode { get; set; }
        public int WorkHour { get; set; }
        public int? WorkerCount { get; set; }
        public int? Absent { get; set; }
        public float? ActualProduction { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string BrandCode { get; set; }
        public double? TotalTargetManual { get; set; }

        public double TargetManual { get; set; }
    }
}
