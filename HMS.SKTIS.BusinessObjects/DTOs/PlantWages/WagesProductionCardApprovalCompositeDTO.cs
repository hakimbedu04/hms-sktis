using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class WagesProductionCardApprovalCompositeDTO
    {
        public int? RevisionType { get; set; }
        public string ProductionCardCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public int? IDFlow { get; set; }
        public string Status { get; set; }
        public string UserAD { get; set; }
        public int IDRole { get; set; }
        public string RolesName { get; set; }
        public int Shift { get; set; }
    }
}
