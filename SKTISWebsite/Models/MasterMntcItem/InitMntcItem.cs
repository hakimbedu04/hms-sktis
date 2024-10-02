using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterMntcItem
{
    public class InitMntcItem
    {
        public List<SelectListItem> ItemTypes { get; set; }
        public List<SelectListItem> UOMs { get; set; }
        public List<SelectListItem> PriceTypes { get; set; }
    }
}