using System.Collections.Generic;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.UtilSecurityRoles
{
    public class InitUtilSecurityRoles
    {
        public SelectList RolesList { get; set; } 
        public List<RolesLookupList> RolesLookupList { get; set; } 
    }
}