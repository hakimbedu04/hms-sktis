using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesEblekRelease
{
    public class ExePlantProductionEntryVerificationViewModel
    {
        public string ProductionEntryCode { get; set; }
        public bool? Checkbox { get; set; }
        public string ProductionDate { get; set; }
        public string BrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string Remark { get; set; }

    }
}