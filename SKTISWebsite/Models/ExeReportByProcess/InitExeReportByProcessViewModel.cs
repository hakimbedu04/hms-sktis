using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeReportByProcess
{
    public class InitExeReportByProcessViewModel
    {
        public InitExeReportByProcessViewModel()
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
        public string Param1LocationCode { get; set; }
        public string Param2UnitCode { get; set; }
        public int? Param3Shift { get; set; }
        public string Param4BrandCode { get; set; }
        public string Param5BrandGroupCode { get; set; }
        public DateTime Param6Date { get; set; }
    }
}