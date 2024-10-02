using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.PlanningPlantIndividualCapacity
{
    public class InitPlanningPlantIndividualCapacityViewModel
    {
        public InitPlanningPlantIndividualCapacityViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultDate = DateTime.Now.Date;
            DefaultDatePreviousThreeWeeks = DateTime.Now.Date.AddDays(-21);
        }

        public List<LocationLookupList> LocationSelectList { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public DateTime DefaultDate { get; set; }
        public DateTime DefaultDatePreviousThreeWeeks { get; set; }
    }
}