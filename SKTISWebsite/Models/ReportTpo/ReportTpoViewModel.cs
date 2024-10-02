using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ReportTpo
{
    public class ReportTpoViewModel : ViewModelBase
    {
        public bool Checkbox { get; set; }
        public int Id { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
    }
}