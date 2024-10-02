using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.WagesEblekRelease
{
    public class InitWagesEblekReleaseViewModel
    {
        public SelectList Date { get; set; }
        public List<LocationLookupList> LocationCode { get; set; }
        public string DefaultLocationCode { get; set; }
        public string DefaultUnit { get; set; }
        public int? DefaultShift { get; set; }
        public SelectList KpsYear { get; set; }
        public int? DefaultYear { get; set; }
        public SelectList KpsWeek { get; set; }
        public int? DefaultWeek { get; set; }
        public List<LocationLookupList> PopupLocationCode { get; set; }
        public string DefaultPopupLocationCode { get; set; }
        public string DefaultPopupUnit { get; set; }
        public int? DefaultPopupShift { get; set; }
        public string LocationFromUrl { get; set; }
        public string DateFromUrl { get; set; }
        public string DateNearestClosingPayrollFrom { get; set; }
        public string DateNearestClosingPayrollTo { get; set; }
        
    }
}