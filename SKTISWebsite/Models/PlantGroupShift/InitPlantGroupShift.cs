using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.PlantGroupShift
{
    public class InitPlantGroupShift : ViewModelBase
    {
        public InitPlantGroupShift()
        {
            DefaultYear = DateTime.Now.Year;
        }

        public List<LocationLookupList> Location { get; set; }      
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<PlanPlantGroupShiftViewModel> ListPlantGroupShift1 { get; set; }
        public List<PlanPlantGroupShiftViewModel> ListPlantGroupShift2 { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public int DefaultYear { get; set; }
    }
}