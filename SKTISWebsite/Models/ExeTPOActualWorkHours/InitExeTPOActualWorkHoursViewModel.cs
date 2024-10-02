using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeTPOActualWorkHours
{
    public class InitExeTPOActualWorkHoursViewModel
    {
        public InitExeTPOActualWorkHoursViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public List<LocationLookupList> TpoLocationLookupLists { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
    }
}