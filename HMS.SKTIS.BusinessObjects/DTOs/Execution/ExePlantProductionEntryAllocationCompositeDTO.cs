using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantProductionEntryAllocationCompositeDTO
    {
        public ExePlantProductionEntryAllocationCompositeDTO()
        {
            Status = true;
        }

        public string ProductionEntryCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public bool Status { get; set; }
    }
}
