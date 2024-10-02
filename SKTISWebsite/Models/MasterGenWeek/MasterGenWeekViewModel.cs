using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenWeek
{
    public class MasterGenWeekViewModel
    {
        public int IdMasterWeek { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? Week { get; set; }
        public string Month { get; set; }
        public int? Year { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}