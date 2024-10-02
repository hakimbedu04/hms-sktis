using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.WagesReportAbsents
{
    public class InitWagesReportAbsentsDetailViewModel
    {
        public InitWagesReportAbsentsDetailViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultMonth = DateTime.Now.Month;
            DefaultDateTo = DateTime.Now.Date;
        }
        public SelectList EmployeeSelectList { get; set; }
        public SelectList LocationSelectList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public IEnumerable<SelectListItem> MonthSelectList { get; set; }
        public string DefaultEmployeeID { get; set; }
        public string DefaultEmployee { get; set; }
        public string DefaultLocation { get; set; }
        public string DefaultLocationName { get; set; }
        public string DefaultUnit { get; set; }
        public string DefaultGroup { get; set; }
        public string DefaultProcess { get; set; }
        public string DefaultBrand { get; set; }
        public int? DefaultYear { get; set; }
        public int? DefaultMonth { get; set; }
        public int? DefaultWeek { get; set; }        
        public DateTime? DefaultDateFrom { get; set; }
        public DateTime? DefaultDateTo { get; set; }
    }
}