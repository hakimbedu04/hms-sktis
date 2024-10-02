using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.MasterMntcItemLocation;

namespace SKTISWebsite.Models.MaintenanceExecutionItemConversion
{
    public class InitMaintenanceExecutionItemConversionViewModel
    {
        public InitMaintenanceExecutionItemConversionViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            //DefaultMonth = DateTime.Now.Month;
        }
        public List<LocationLookupList> Locations { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public int DefaultYear { get; set; }
        //public int DefaultMonth { get; set; }
        public int? DefaultWeek { get; set; }
        public string TodayDate { get; set; }
        public List<SelectListItem> Months { get; set; }
        public List<MstMntcItemDescription> ItemDescriptions { get; set; }
        public List<MasterItemConversionComposite> MsterItemConversionComposites { get; set; }
    }

    public class MasterItemConversionComposite
    {
        public string ItemCodeSource { get; set; }
        public string ItemCodeDestination { get; set; }
        public bool ConversionType { get; set; }
        public int? QtyConvert { get; set; }
    }
}