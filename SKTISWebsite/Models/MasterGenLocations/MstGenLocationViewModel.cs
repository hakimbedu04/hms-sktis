using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterGenLocations
{
    public class MstGenLocationViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string CostCenter { get; set; }
        public string ABBR { get; set; }
        public int Shift { get; set; }
        public string ParentLocationCode { get; set; }
        public int UMK { get; set; }
        public string KPPBC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public bool StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}