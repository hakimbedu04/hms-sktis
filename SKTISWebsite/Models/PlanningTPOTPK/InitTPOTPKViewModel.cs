using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.PlanningTPOTPK
{
    public class InitTPOTPKViewModel
    {
        public InitTPOTPKViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public string TodayDate { get; set; }
        public int DefaultTPK { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultBrandCode { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> TPOChildLocationLookupList { get; set; }
        public float DefaultTargetWPP { get; set; }
    }
}