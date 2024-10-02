using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExeEMSSourceData
{
    public class InitExeEMSSourceDataViewModel
    {
        public List<SelectListItem> PLNTChildLocation { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
        public string defaultLocation { get; set; }
    }
}