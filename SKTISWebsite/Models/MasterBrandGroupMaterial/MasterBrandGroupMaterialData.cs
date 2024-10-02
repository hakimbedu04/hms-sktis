using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterBrandGroupMaterial
{
    public class MasterBrandGroupMaterialData : ViewModelBase
    {
        public string MaterialCode { get; set; }
        public string OldMaterialCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public string Uom { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }
}