using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using HMS.SKTIS.Core;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.PlanningPlantTPU
{
    public class InitPlantTPUViewModel
    {
        public InitPlantTPUViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultShift = 1;
            CurrentDayForward = DateTime.Now.Date.AddDays(1);
            DefaultConversion = Enums.Conversion.Box.ToString();
        }

        public int DefaultYear { get; set; }
        public string TodayDate { get; set; }
        public int DefaultShift { get; set; }
        public string DefaultConversion { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultBrandCode { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public SelectList ConversionSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public float DefaultTargetWPP { get; set; }
        public DateTime CurrentDayForward { get; set; }
        // Property GET URL
        public string LocationCode { get; set; }
        public string Brand { get; set; }
        public int Shift { get; set; }
        public string Conversion { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime CurrentDayFoward { get; set; }
    }
}
