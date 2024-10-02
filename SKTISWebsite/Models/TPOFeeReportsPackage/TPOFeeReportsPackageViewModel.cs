using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.TPOFeeReportsPackage
{
    public class TPOFeeReportsPackageViewModel
    {
        public TPOFeeReportsPackageViewModel()
        {
            ListWeekValue = new List<TPOFeeReportsPackageWeeklyViewModel>();  
        }


        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string BrandCode { get; set; }
        public string MemoReff { get; set; }

        public List<TPOFeeReportsPackageWeeklyViewModel> ListWeekValue { get; set; }

        public int Year { get; set; }
        public int MaxWeek { get; set; }

    }

    public class TPOFeeReportsPackageWeeklyViewModel
    {
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string PackageValue { get; set; }
        public bool IsChangedValue { get; set; }
    }
}