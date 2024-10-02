using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MaintenanceExecutionInventory
{
    public class InitMaintenanceExecutionInventoryViewModel
    {
        public List<LocationLookupList> LocationLookupList { get; set; }
        public string CurrentDate { get; set; }
        public SelectList ItemTypes { get; set; }
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
