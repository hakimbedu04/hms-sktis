using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EblekReleaseApproval
{
    public class EblekReleaseApprovalViewModel : ViewModelBase
    {
        public string ProductionEntryCode { get; set; }
        public bool IsLocked { get; set; }
        public string EblekDate { get; set; }
        public string BrandCode { get; set; }
        public string GroupCode { get; set; }
        public string Remark { get; set; }
        public bool Flag { get; set; }
        public string OldValueRemark { get; set; }
    }
}