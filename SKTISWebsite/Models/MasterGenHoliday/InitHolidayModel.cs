using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterGenLocations;

namespace SKTISWebsite.Models.MasterGenHoliday
{
    public class InitHolidayModel : ViewModelBase
    {
        public List<SelectListItem> LocationCode { set; get; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }
        public List<SelectListItem> Years { get; set; }
        public List<SelectListItem> HolidayType { set; get; }
        public DateTime HolidayDate { get; set; }
        public string CurrentLocation { get; set; }
    }
}