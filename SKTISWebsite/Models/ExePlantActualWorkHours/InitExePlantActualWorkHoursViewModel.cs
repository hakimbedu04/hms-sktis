using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExePlantActualWorkHours
{
    public class InitExePlantActualWorkHoursViewModel
    {
        public InitExePlantActualWorkHoursViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
    }
}