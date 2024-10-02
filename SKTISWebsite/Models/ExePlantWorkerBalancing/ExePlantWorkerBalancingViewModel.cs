using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKTISWebsite.Models.LookupList;


namespace SKTISWebsite.Models.ExePlantWorkerBalancing 
{
    public class ExePlantWorkerBalancingViewModel :ViewModelBase
    {
        //public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public string TransactionDate { get; set; }
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
        public string EmployeeMix { get; set; }
    }
}