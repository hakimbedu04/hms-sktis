using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.UtilSecurityRules
{
    public class UtilSecurityRulesViewModel : ViewModelBase
    {
        public int IDRule { get; set; }
        public string RulesName { get; set; }
        public string Location { get; set; }
        public string Unit { get; set; }
        public int IDResponsibility { get; set; }
    }

    public class UtilSecurityRulesUnitViewModel : ViewModelBase
    {
        public int IDRule { get; set; }
        public string RulesName { get; set; }
        public string Location { get; set; }
        public string Unit { get; set; }
    }
}