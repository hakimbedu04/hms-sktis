using System;
using System.Collections.Generic;
using System.Linq;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentReceive
{
    public class InitEquipmentReceiveViewModel
    {
        public InitEquipmentReceiveViewModel()
        {
            DefaultTransferDate = DateTime.Now;
        }

        public DateTime DefaultTransferDate { get; set; }
        public List<LocationLookupList> LocationLookupList { get; set; }
        public List<LocationLookupList> SourceLocationLookupList { get; set; }

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

        public string DefaultLocationSource
        {
            get 
            {
                var defaultLocationSource = string.Empty;
                var locationLookupListSource = SourceLocationLookupList.FirstOrDefault();
                if (locationLookupListSource != null)
                {
                    defaultLocationSource = locationLookupListSource.LocationCode;
                }
                return defaultLocationSource;
            }
        }
    }
}
