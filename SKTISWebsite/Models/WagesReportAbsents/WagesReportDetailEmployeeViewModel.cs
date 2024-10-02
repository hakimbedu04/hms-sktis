using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesReportAbsents
{
    public class WagesReportDetailEmployeeViewModel
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeID { get; set; }
        public string ProductionDate { get; set; }
        public string AbsentType { get; set; }
        public string EmployeeName { get; set; }
    }
}