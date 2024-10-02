using System;
using System.Collections.Generic;
using System.Linq;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentTransfer
{
    public class InitEquipmentTransferViewModel
    {
        public InitEquipmentTransferViewModel()
        {
            DefaultTransferDate = DateTime.Now;
        }

        public DateTime DefaultTransferDate { get; set; }
        public List<LocationLookupList> LocationLookupList { get; set; }
        public List<LocationLookupList> LocationDestinationLookupList { get; set; }

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

        public string DefaultLocationDestination
        {
            get
            {
                var defaultLocationCode = string.Empty;
                var locationLookupList = LocationDestinationLookupList.FirstOrDefault();
                if (locationLookupList != null)
                {
                    defaultLocationCode = locationLookupList.LocationCode;
                }
                return defaultLocationCode;
            }
        }

    }
}
