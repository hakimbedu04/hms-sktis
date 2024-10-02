using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenLocations
{
    public class MasterGenLocationDesc : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string ParentLocationCode { get; set; }
    }
}