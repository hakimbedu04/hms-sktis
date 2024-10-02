using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterGenBrandGroup
{
    public class IndexMasterBrandGroupViewModel
    {
        public SelectList BrandFamily { get; set; }
        public SelectList BrandGroupCode { get; set; }
        public SelectList PackType { get; set; }
        public SelectList ClassType { get; set; }

        //public List<BrandGroupModel> Details { get; set; } 
    }
}