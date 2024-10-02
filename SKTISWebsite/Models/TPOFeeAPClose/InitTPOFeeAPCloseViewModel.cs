using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SKTISWebsite.Models.TPOFeeAPClose
{
    public class InitTPOFeeAPCloseViewModel
    {
        public InitTPOFeeAPCloseViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList Regional { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
    }
}
