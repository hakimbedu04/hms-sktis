using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ReportPlan
{
    public class ReportPlanModel : ViewModelBase
    {
        [Required]
        public string ReportName { get; set; }
        [Required]
        public string ReportUrl { get; set; }
    }
}