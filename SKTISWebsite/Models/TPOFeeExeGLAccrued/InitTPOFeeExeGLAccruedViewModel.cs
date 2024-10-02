using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.TPOFeeExeGLAccrued
{
    public class InitTPOFeeExeGLAccruedViewModel
    {
        public InitTPOFeeExeGLAccruedViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList Regional { get; set; }
        public SelectList ClosingDate { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }

        public string DefaultRegional { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
    }
}