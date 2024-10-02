using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKTISWebsite.Models.LookupList;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExePlantWorkerBalancing
{
    public class InitExePlantWorkerBalancing
    {
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public string DefaultLocationCode { get; set; }
        public SelectList FilterLocation { get; set; }
    }
}