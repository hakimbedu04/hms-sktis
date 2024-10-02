using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PagedList;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.MasterGenLocations;

namespace SKTISWebsite.Models.EquipmentRequest
{
    public class InitEquipmentRequest : GridBaseViewModel
    {
        public string FilterLocationCode { get; set; }
        public string FilterRequestDate { get; set; }
        public List<LocationLookupList> LocationCodeSelectList { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }
    }
}