using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.TPOFeeExePlan
{
    public class InitTPOFeeExePlanViewModel
    {
        public SelectList Regional { get; set; }
        public SelectList KpsWeek { get; set; }
        public SelectList KpsYear { get; set; }
        public string DefaultRegional { get; set; }
        public int? DefaultKpsWeek { get; set; }
        public int? DefaultKpsYear { get; set; }
    }
}