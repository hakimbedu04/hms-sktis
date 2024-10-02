using System;

namespace SKTISWebsite.Models.UtilSecurityDelegations
{
    public class UtilSecurityDelegationsViewModel : ViewModelBase
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Responsibility { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public int IDResponsibility { get; set; }

        public string UserADToOld { get; set; }
        public int IDResponsibilityOld { get; set; }
        public string UserADFromOld { get; set; }
        public string StartDateOldString { get; set; }
    }
}