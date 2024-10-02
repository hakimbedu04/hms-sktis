using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesReportAvailablePositionNumber
{
    public class ExeWagesReportAvailablePositionNumberViewModel : ViewModelBase 
    {
        public string LocationCode { get; set; }
        public string EmployeeNumber { get; set; }
        public string GroupCode { get; set; }
        public string Status { get; set; }
        public string UnitCode { get; set; }
        public string ProcessSettingsCode { get; set; }
    }
}