//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HMS.SKTIS.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    
    public partial class UtilRole
    {
        public UtilRole()
        {
            this.UtilResponsibilities = new HashSet<UtilResponsibility>();
            this.UtilRolesFunctions = new HashSet<UtilRolesFunction>();
        }
    
        public int IDRole { get; set; }
        public string RolesCode { get; set; }
        public string RolesName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<UtilResponsibility> UtilResponsibilities { get; set; }
        public virtual ICollection<UtilRolesFunction> UtilRolesFunctions { get; set; }
    }
}
