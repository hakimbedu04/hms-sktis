using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterGenLocations;

namespace SKTISWebsite.Models.MstTPOPackage
{
    public class InitMstTPOPackageViewModel
    {
        public List<SelectListItem> Locations { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }
        public List<SelectListItem> BrandGroups { get; set; }
        public List<SelectListItem> Years { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string UploadPath { get; set; }
    }
}