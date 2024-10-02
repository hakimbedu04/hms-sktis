using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MasterGenProcess;

namespace SKTISWebsite.Models.MasterGenProccessSetting
{
    public class InitMasterGenProccessSetting
    {
        public SelectList LocationCodeSelectList { get; set; }
        public SelectList BrandGroupCodeSelectList { get; set; }
        public SelectList ProcessGroupSelectList { get; set; }        
        public List<MasterGenProcessIdentifier> ProcessList { get; set; }
        public List<LocationLookupList> LocationNameLookupList { get; set; } 
    }
}