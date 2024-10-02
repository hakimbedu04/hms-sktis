using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilResponsibilityDTO
    {
        public int IDResponsibility { get; set; }
        public Nullable<int> IDRole { get; set; }
        public string RolesName { get; set; }
        public string ResponsibilityName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ExpiredDate { get; set; }
    }
}
