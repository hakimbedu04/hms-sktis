using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantWorkerAssignmentDTO
    {
        public System.DateTime TransactionDate { get; set; }
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string KPSYear { get; set; }
        public string KPSWeek { get; set; }
        public string OldEmployeeID { get; set; }
        public DateTime OldStartDate { get; set; }
        public DateTime OldEndDate { get; set; }
    }
}
