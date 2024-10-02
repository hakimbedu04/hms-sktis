using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterBrand;

namespace SKTISWebsite.Models.PlanningWPP
{
    public class InitPlanningWPPModel : ViewModelBase
    {
        public List<SelectListItem> KPSYears { get; set; }
        public List<SelectListItem> KPSWeeks { get; set; }
        public List<SelectListItem> LocationCodes { set; get; }
        public List<BrandCodeModel> BrandCodes { set; get; }
        public SelectList BrandFamily { get; set; }
        public WPPResultModel WPPResult { get; set; }
        public int? DefaultWeek { get; set; }
    }
}