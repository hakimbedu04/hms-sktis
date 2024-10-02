using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.MasterGenLocations;

namespace SKTISWebsite.Models.MasterTOPInfo
{
    public class InitMasterTPOInfo
    {
        public List<SelectListItem> ItemLocationCodes { get; set; }
        public List<SelectListItem> ItemTPORanks { get; set; }
        public List<MasterGenLocationDesc> LocationDescs { get; set; }
    }
}