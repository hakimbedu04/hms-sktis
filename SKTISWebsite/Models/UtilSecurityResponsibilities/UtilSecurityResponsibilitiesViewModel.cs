using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.UtilSecurityResponsibilities
{
    public class UtilSecurityResponsibilitiesViewModel : ViewModelBase
    {
        public int IDResponsibility { get; set; }
        public Nullable<int> IDRole { get; set; }
        public string ResponsibilityName { get; set; }
        public string RolesName { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class UtilSecurityUsersResponsibilitiesViewModel : ViewModelBase
    {
        public int IDResponsibility { get; set; }
        public string UserAD { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class UtilUsersResponsibilitiesRoleViewModel : ViewModelBase
    {
        public int IDResponsibility { get; set; }
        public string ResponsibilityName { get; set; }
        public string UserAD { get; set; }
        public string RolesCode { get; set; }
        public string RolesName { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}