using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenStandardHour
{
    public class MasterGenStandardHourViewModel : ViewModelBase
    {
        public string DayType { get; set; }
        public string DayName { get; set; }
        public int? JknHour { get; set; }
        public int? Jl1Hour { get; set; }
        public int? Jl2Hour { get; set; }
        public int? Jl3Hour { get; set; }
        public int? Jl4Hour { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}