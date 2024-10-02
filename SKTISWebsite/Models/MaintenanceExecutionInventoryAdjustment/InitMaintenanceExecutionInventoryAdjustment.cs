using HMS.SKTIS.BusinessObjects.DTOs;
using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MaintenanceExecutionInventoryAdjustment
{
    public class InitMaintenanceExecutionInventoryAdjustment
    {
        public InitMaintenanceExecutionInventoryAdjustment()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public string TodayDate { get; set; }
        public List<LocationLookupList> LocationLookupList { get; set; }

        public List<MstMntcItemCompositeDTO> ItemsList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public string DefaultUnitCode { get; set; }
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