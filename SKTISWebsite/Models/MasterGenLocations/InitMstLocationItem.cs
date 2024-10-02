using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MasterGenLocations
{
    public class InitMstLocationItem
    {
        public List<LocationLookupList> LocationLookupList { get; set; }
        public List<SelectListItem> ItemShift { get; set; }
        public List<SelectListItem> ItemLocationCodes { get; set; }
        public List<SelectListItem> ItemTPORanks { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }

        public string DefaultLocation
        {
            get
            {
                var defaultLocationCode = string.Empty;
                var locationLookupList = LocationLookupList.FirstOrDefault();
                if (locationLookupList != null)
                {
                    defaultLocationCode = locationLookupList.LocationCode;
                }
                return defaultLocationCode;
            }
        }
    }
}