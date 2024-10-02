using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ProductAdjustmentDTO
    {
        public DateTime ProductionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public int Shift { get; set; }
        public string BrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public string AdjustmentType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? AdjustmentValue { get; set; }
        public string AdjustmentRemark { get; set; }
        public string StatusEmp { get; set; }
        public string StatusIdentifier { get; set; }
        public string GroupCode { get; set; }
    }
}
