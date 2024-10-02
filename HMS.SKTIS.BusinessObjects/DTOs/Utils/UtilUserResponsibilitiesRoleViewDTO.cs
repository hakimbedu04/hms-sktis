using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilUserResponsibilitiesRoleViewDTO
    {
        public int IDResponsibility { get; set; }
        public string ResponsibilityName { get; set; }
        public string UserAD { get; set; }
        public string RolesCode { get; set; }
        public string RolesName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
