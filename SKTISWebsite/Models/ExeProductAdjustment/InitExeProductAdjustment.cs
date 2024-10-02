using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeProductAdjustment
{
    public class InitExeProductAdjustment
    {
        public InitExeProductAdjustment()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultProductionDate = DateTime.Now.Date;
        }

        public List<LocationLookupList> LocationSelectList { get; set; }
        public string DefaultLocation { get; set; }
        public SelectList YearSelectList { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public DateTime DefaultProductionDate { get; set; }
    }
}