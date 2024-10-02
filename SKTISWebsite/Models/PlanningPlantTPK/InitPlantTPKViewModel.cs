using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.PlanningPlantTPK
{
    public class InitPlantTPKViewModel
    {
        public InitPlantTPKViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            CurrentDayForward = DateTime.Now.Date.AddDays(1);
        }

        public int DefaultYear { get; set; }
        public string TodayDate { get; set; }
        public string DefaultUnitCode { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultBrandCode { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public DateTime CurrentDayForward { get; set; }
        public float DefaultTargetWPP { get; set; }

        public string Param1LocationCode { get; set; }
        public string Param2UnitCode { get; set; }
        public string Param3BrandCode { get; set; }
        public int? Param4Shift { get; set; }
        public int? Param5KPSYear { get; set; }
        public int? Param6KPSWeek { get; set; }

    }
}
