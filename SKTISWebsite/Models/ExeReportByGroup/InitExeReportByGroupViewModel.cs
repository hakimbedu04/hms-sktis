using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExeReportByGroup
{
    public class InitExeReportByGroupViewModel
    {
        public InitExeReportByGroupViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultMonth = DateTime.Now.Month;
        }
        public SelectList AbsentTypes { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public int? DefaultMonth { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public IEnumerable<SelectListItem> MonthSelectList { get; set; }
        public IEnumerable<SelectListItem> MonthToSelectList { get; set; }

    }
}
