using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ExePlantWorkerAssignment
{
    public class ExePlantWorkerAssignmentViewModel : ViewModelBase
    {
        public string TransactionDate { get; set; }
        public string SourceLocationCode { get; set; }
        public string SourceUnitCode { get; set; }
        public int? SourceShift { get; set; }
        public string SourceProcessGroup { get; set; }
        public string SourceGroupCode { get; set; }
        public string SourceBrandCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string DestinationUnitCode { get; set; }
        public int? DestinationShift { get; set; }
        public string DestinationProcessGroup { get; set; }
        public string DestinationGroupCode { get; set; }
        public string DestinationGroupCodeDummy { get; set; }
        public string DestinationBrandCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string OldEmployeeID { get; set; }
        public string OldStartDate { get; set; }
        public string OldEndDate { get; set; }
    }
}
