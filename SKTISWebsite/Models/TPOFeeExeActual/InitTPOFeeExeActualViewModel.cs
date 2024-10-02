using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.TPOFeeExeActual
{
    public class InitTPOFeeExeActualViewModel
    {
        public InitTPOFeeExeActualViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList Regional { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }

        public string Param1LocationCode { get; set; }
        public int? Param2Year { get; set; }
        public int? Param3Week { get; set; }
    }
}
