using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantWorkerBalancingViewDTO
    {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string SourceGroupcode { get; set; }
        public string SourceUnitCode { get; set; }
        public string SourceLocationCode { get; set; }
        public string SourceProcess { get; set; }
        public string DestinationProcess { get; set; }
        public string UnitCodeDestination { get; set; }
        public string GroupCodeDestination { get; set; }
        public bool? statfrom { get; set; }
        public bool? Checkbox { get; set; }
    }
}
