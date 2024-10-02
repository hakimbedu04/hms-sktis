using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterGenHoliday
{
    public class MasterGenHolidayViewModel : ViewModelBase
    {
        public string HolidayDate { get; set; }
        public string HolidayType { get; set; }
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}