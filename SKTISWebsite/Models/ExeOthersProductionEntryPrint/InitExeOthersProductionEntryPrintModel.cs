using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExeOthersProductionEntryPrint
{
    public class InitExeOthersProductionEntryPrintModel
    {
        public InitExeOthersProductionEntryPrintModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public List<LocationLookupList> LocationLookupList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
    }
}