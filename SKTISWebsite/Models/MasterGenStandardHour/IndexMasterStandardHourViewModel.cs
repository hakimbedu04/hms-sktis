using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterGenStandardHour
{
    public class IndexMasterStandardHourViewModel
    {
        public SelectList DayType { get; set; }
        public SelectList Day { get; set; } 
    }
}