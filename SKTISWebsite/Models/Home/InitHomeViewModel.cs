using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.Home
{
    public class InitHomeViewModel
    {
        public InitHomeViewModel()
        {
            IsExpired = false;
            IsRuleEmpty = false;
        }

        public bool IsRuleEmpty { get; set; }
        public bool IsExpired { get; set; }
    }
}