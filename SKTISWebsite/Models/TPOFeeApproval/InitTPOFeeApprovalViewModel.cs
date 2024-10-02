using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SKTISWebsite.Models.TPOFeeApproval
{
    public class InitTPOFeeApprovalViewModel
    {
        public InitTPOFeeApprovalViewModel()
        {
            DefaultYear = DateTime.Now.Year;
        }
        public SelectList Regional { get; set; }
        public int DefaultYear { get; set; }
        public int? DefaultWeek { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public string BackToList { get; set; }
        public string Param2 { get; set; }
        public bool DisableBtnBack { get; set; }

        public string PageFrom { get; set; }
    }
}
