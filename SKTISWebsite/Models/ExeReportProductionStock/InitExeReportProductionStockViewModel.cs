using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeReportProductionStock
{
    public class InitExeReportProductionStockViewModel
    {
        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentWeek { get; set; }
        public string CurrentDay { get; set; }
        public IEnumerable<SelectListItem> ListYear { get; set; }
        public IEnumerable<SelectListItem> ListMonth { get; set; }
        public IEnumerable<SelectListItem> ListMonthTo { get; set; }
        public List<LocationLookupList> LocationCodeSelectList { get; set; }
    }
}