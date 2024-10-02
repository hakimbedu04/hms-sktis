using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilRoleDTO
    {
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
