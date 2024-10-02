using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExeReportDailyProductionAchievement
{
    public class InitExeReportDailyProductionAchievementViewModel
    {
        public InitExeReportDailyProductionAchievementViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            //DefaultMonth = DateTime.Now.Month;
            Details = new ExeReportDailyProductionAchievementViewModel();

        }
        
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> LocationSelectList { get; set; }
        public List<DateTime> DateList { get; set; }

        public ExeReportDailyProductionAchievementViewModel Details { get; set; }

        public string Param1LocationCode { get; set; }
        public int? Param2Year { get; set; }
        public int? Param3Week { get; set; }
        
    }

   
}