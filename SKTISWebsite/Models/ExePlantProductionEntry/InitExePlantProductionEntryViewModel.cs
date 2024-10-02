using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExePlantProductionEntry
{
    public class InitExePlantProductionEntryViewModel
    {
        public InitExePlantProductionEntryViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList AbsentTypes { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public string DefaultBrandCode { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public List<AbsentTypeLookupList> AbsentTypeLookupLists { get; set; }
        public string Param1LocationCode { get; set; }
        public string Param2UnitCode { get; set; }
        public int? Param3Shift { get; set; }
        public string Param4ProcessGroup { get; set; }
        public string Param5GroupCode { get; set; }
        public string Param6BrandCode { get; set; }
        public int? Param7KPSYear { get; set; }
        public int? Param8KPSWeek { get; set; }
        public DateTime Param9ProductionDate { get; set; }
    }
}
