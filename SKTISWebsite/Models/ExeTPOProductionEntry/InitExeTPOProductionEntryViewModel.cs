using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExeTPOProductionEntry
{
    public class InitExeTPOProductionEntryViewModel
    {
        public InitExeTPOProductionEntryViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultDate = DateTime.Now.Date.Date;
        }
        public DateTime? DefaultDate { get; set; }
        public SelectList AbsentTypes { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }

        public string Param1LocationCode { get; set; }
        public string Param2ProcessGroup { get; set; }
        public string Param3StatusEmp { get; set; }
        public string Param4BrandCode { get; set; }
        public int? Param5KPSYear { get; set; }
        public int? Param6KPSWeek { get; set; }
        public DateTime Param7Date { get; set; }
    }
}
