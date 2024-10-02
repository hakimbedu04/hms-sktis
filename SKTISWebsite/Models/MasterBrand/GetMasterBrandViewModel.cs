using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterBrand
{
    public class GetMasterBrandViewModel : ViewModelBase
    {
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string Description { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string Remark { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}