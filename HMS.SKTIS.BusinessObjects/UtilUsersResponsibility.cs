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
    
    public partial class UtilUsersResponsibility
    {
        public UtilUsersResponsibility()
        {
            this.UtilDelegations = new HashSet<UtilDelegation>();
        }
    
        public int IDResponsibility { get; set; }
        public string UserAD { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstADTemp MstADTemp { get; set; }
        public virtual ICollection<UtilDelegation> UtilDelegations { get; set; }
        public virtual UtilResponsibility UtilResponsibility { get; set; }
    }
}
