using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.WagesProductionCardApproval
{
    public class InitWagesProductionCardApprovalViewModel
    {
        public IEnumerable<SelectListItem> WeekList { get; set; }
        public int Month = DateTime.Now.Month;
        public int? Week { get; set; }
        public DateTime Date = DateTime.Now.Date;
    }
}