using System;
using System.Collections.Generic;
using SKTISWebsite.Models.MasterGenBrandGroup;

namespace SKTISWebsite.Models.MasterGenBrandPackageMapping
{
    public class InitBrandPackageMapping
    {
        public List<MasterGenBrandGroupViewModel> BrandGroupCodes { get; set; }
        public List<BrandPkgMappingViewModel> BrandPackageMapping { get; set; }
    }
}