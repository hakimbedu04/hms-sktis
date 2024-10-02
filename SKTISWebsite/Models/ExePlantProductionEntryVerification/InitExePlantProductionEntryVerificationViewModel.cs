using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExePlantProductionEntryVerification
{
    public class InitExePlantProductionEntryVerificationViewModel
    {
        public InitExePlantProductionEntryVerificationViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList AbsentTypes { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public string Param1LocationCode { get; set; }
        public string Param2UnitCode { get; set; }
        public int? Param3Shift { get; set; }
        public string Param4BrandCode { get; set; }
        public int? Param5KPSYear { get; set; }
        public int? Param6KPSWeek { get; set; }
        public DateTime Param7Date { get; set; }
    }
}
