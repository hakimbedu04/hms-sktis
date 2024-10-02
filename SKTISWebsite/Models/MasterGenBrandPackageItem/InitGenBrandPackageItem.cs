using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterMntcItemLocation;

namespace SKTISWebsite.Models.MasterGenBrandPackageItem
{
    public class InitGenBrandPackageItem
    {
        public List<BrandCodeSelectItem> BrandGroup { get; set; }
        public List<SelectListItem> Item { get; set; }
        public List<MstMntcItemDescription> ItemDescription { get; set; }
    }
}