using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExePlantWorkerAssignment
{
    public class InitExePlantWorkerAssignmentViewModel
    {
        public InitExePlantWorkerAssignmentViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public List<LocationLookupList> PLNTTPOChildLocationLookupList { get; set; }
        public string TodayDate { get; set; }
        public string ClosingPayrollDate { get; set; }
    }
}
