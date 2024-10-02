using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.WagesReportAvailablePositionNumber
{
    public class InitWagesReportAvailablePositionNumberViewModel
    {
        public List<SelectListItem> PLNTChildLocation { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
        public DateTime? DefaultDate { get; set; }
    }
}