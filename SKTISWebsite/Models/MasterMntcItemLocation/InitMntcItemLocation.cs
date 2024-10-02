using SKTISWebsite.Models.LookupList;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterMntcItemLocation
{
    public class InitItemLocationViewModel
    {
        public List<SelectListItem> Item { get; set; }
        public List<SelectListItem> Location { get; set; }
        public List<MstMntcItemDescription> ItemDescription { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
    }
}