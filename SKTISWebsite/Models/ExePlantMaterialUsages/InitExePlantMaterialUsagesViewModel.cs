using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.ExePlantMaterialUsages
{
    public class InitExePlantMaterialUsagesViewModel
    {
        public InitExePlantMaterialUsagesViewModel()
        {
            DefaultYear = DateTime.Now.Year;
            DefaultDate = DateTime.Now.Date.Date;
        }

        public DateTime? DefaultDate { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
    }
}
