using SKTISWebsite.Models.LookupList;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SKTISWebsite.Models.EblekReleaseApproval
{
    public class InitEblekRelaseApproval
    {
        public SelectList LocationCodeSelectList { get; set; }
        public List<LocationLookupList> PlntChildLocationLookupList { get; set; }
        public string TodayDate { get; set; }
        public int? DefaultWeek { get; set; }
        public int DefaultYear { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public SelectList Date { get; set; }


        public string Param1LocationCode { get; set; }
        public string Param2UnitCode { get; set; }
        public int? Param3Shift { get; set; }
        public string Param4Process { get; set; }
        public string Param5Date { get; set; }

        public string dateFromUrl { get; set; }

        public int weekFromUrl { get; set; }

        public int yearFromUrl { get; set; }
        


    }
}