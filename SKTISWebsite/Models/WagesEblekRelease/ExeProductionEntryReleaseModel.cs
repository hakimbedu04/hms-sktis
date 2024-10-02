using SKTISWebsite.Models.ExePlantProductionEntryVerification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesEblekRelease
{
    public class ExeProductionEntryReleaseModel : ViewModelBase
    {
        public bool? Checkbox { get; set; }
        public string ProductionEntryCode { get; set; }
        public string Remark { get; set; }
        public bool? IsLocked { get; set; }

        public ExePlantProductionEntryVerificationViewModel ExePlantProductionEntryVerification { get; set; }
    }
}