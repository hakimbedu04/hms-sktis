using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class MaintenanceExecutionInventoryAdjustmentDTO
    {
        public DateTime AdjustmentDate { get; set; }
        public string UnitCode { get; set; }
        public string UnitCodeDestination { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemCodeDescription { get; set; }
        public string ItemStatusFrom { get; set; }
        public string ItemStatusTo { get; set; }
        public int AdjustmentValue { get; set; }
        public string AdjustmentType { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
