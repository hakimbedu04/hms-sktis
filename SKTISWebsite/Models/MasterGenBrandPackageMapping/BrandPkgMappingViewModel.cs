using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenBrandPackageMapping
{
    public class BrandPkgMappingViewModel : ViewModelBase
    {
        public string BrandGroupCodeSource { get; set; }
        public string BrandGroupCodeDestination { get; set; }
        public string MappingValue { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}