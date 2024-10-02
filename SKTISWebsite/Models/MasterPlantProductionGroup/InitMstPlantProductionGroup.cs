using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MasterPlantProductionGroup
{
    public class InitMstPlantProductionGroup
    {
        public SelectList LocationCodeSelectList { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; } 
        public List<MandorsLookupList> LeaderLookupList { get; set; }
        public SelectList GroupCodeSelectList { get; set; }
    }
}