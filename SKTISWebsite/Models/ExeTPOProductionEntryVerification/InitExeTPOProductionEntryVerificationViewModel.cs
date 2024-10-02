using HMS.SKTIS.Core;
using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeTPOProductionEntryVerification
{
    public class InitExeTPOProductionEntryVerificationViewModel
    {
        public InitExeTPOProductionEntryVerificationViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultDate = DateTime.Now.Date.ToString(Constants.DefaultDateOnlyFormat);
        }
        public List<LocationLookupList> LocationLookupList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }                
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultDate { get; set; }
        public bool CanSubmit { get; set; }
        public bool CanCancelSubmit { get; set; }
    }
}