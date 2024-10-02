using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilUsersResponsibilityDTO
    {
        public int IDResponsibility { get; set; }
        public string UserAD { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
