using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MaintenanceEquipmentQualityInspection
{
    public class InitMaintenanceExecutionQualityInspection
    {
        public List<LocationLookupList> LocationLookupList { get; set; }
        public List<LocationLookupList> Locations { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
        public SelectList RequestNumber { get; set; }
        public List<MstMntcItemCompositeDTO> ItemsList { get; set; }
        public string Param1LocationCode { get; set; }
        public string Param2 { get; set; }

        public string DefaultLocation
        {
            get
            {
                var defaultLocationCode = string.Empty;
                var locationLookupList = Locations.FirstOrDefault();
                if (locationLookupList != null)
                {
                    defaultLocationCode = locationLookupList.LocationCode;
                }
                return defaultLocationCode;
            }
        }
    }
}