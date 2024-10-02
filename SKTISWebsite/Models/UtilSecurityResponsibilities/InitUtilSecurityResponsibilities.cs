using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.UtilSecurityRules;

namespace SKTISWebsite.Models.UtilSecurityResponsibilities
{
    public class InitUtilSecurityResponsibilities : ViewModelBase
    {
        public List<UtilSecurityRulesViewModel> ListAllRules { get; set; }
        public List<UtilSecurityRulesViewModel> ListActiveRules { get; set; }
        public List<UtilSecurityResponsibilitiesViewModel> ListAllResponsibilities { get; set; }
        public List<UtilSecurityResponsibilitiesViewModel> ListActiveResponsibilities { get; set; }
        public SelectList UtilRoles { get; set; } 
    }
}