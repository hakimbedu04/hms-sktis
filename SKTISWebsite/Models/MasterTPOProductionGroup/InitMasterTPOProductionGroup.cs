using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MasterTPOProductionGroup
{
    public class InitMasterTPOProductionGroup
    {
        public SelectList LocationLists { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }

        public List<LocationLookupList> LocationLookupList { get; set; }

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