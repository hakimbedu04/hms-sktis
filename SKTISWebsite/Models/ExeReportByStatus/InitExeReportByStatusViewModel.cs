using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeReportByStatus
{
    public class InitExeReportByStatusViewModel
    {
        public InitExeReportByStatusViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultMonth = DateTime.Now.Month;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public int? DefaultMonth { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public IEnumerable<SelectListItem> MonthSelectList { get; set; }
        public IEnumerable<SelectListItem> MonthToSelectList { get; set; }
    }
}