using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetProductionCardApprovalDetailInput : BaseInput
    {
        public int? RevisionType { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string Shift { get; set; }
        public string BrandGroupCode { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string BrandCode { get; set; }
    }
}
