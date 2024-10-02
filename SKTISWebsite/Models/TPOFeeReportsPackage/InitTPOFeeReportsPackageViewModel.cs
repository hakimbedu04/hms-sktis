using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Models.TPOFeeReportsPackage
{
    public class InitTPOFeeReportsPackageViewModel
    {
        public InitTPOFeeReportsPackageViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public int DefaultYear { get; set; }

        public IEnumerable<SelectListItem> YearSelectList { get; set; }

       
    }


}