using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.TPOFeeExeAPOpen
{
    public class InitTPOFeeExeAPOpenViewModel
    {
        public InitTPOFeeExeAPOpenViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList Regional { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }

        public string DefaultRegional { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public int roleP1Template { get; set; }
    }
}
