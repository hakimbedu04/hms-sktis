using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExePlantWorkerBalancing
{
    public class ExePlantWorkerSingleBalancingViewModel:ViewModelBase
    {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string LocationCode { get; set; }
        public string UnitCodeSource { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCodeDestination { get; set; }
        public string GroupCodeDestination { get; set; }
        public bool? statfrom { get; set; }
        public bool? Checkbox { get; set; }
        
    }
}