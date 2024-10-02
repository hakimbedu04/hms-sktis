using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningWPP
{
    public class WPPResultModel
    {
        public List<int> Weeks { get; set; }
        public List<WPP13WeekModel> WeeklyProductionPlannings { get; set; }


    }
}