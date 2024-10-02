using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.LookupList;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Models.MaintenanceItemDisposal
{
    public class InitItemDisposal
    {
        public InitItemDisposal()
        {
            DefaultMonth = DateTime.Now.Month;
            DefaultYear = DateTime.Now.Year;
        }
        public List<LocationLookupList> FilterLocation { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<MstMntcItemCompositeDTO> itemDesc { get; set; }
        public SelectList FilterMonth { get; set; }
        public SelectList FilterWeek { get; set; }
        public SelectList FilterItemCode { get; set; }
        public DateTime DateNow { get; set; }

        public int DefaultMonth { get; set; }
        public int? DefaultWeek { get; set; }
        public int? DefaultYear { get; set; }
    }
}