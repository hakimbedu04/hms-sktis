using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenBrandPackageItem
{
    public class MasterGenBrandPackageItemViewModel : ViewModelBase
    {
        public string BrandGroupCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? Qty { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}