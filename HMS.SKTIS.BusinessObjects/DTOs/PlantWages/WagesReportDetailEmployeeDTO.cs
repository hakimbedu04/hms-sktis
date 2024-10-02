using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class WagesReportDetailEmployeeDTO
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeID { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public string AbsentType { get; set; }
        public string EmployeeName { get; set; }
    }
}
