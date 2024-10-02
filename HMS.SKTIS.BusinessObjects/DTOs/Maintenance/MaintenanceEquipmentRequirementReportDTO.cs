using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class MaintenanceEquipmentRequirementReportDTO
    {
        public string BrandGroupCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? realStockQty { get; set; }
        public int Qty { get; set; }
        public float? currentQty { get; set; }
        public float? calculateQty { get; set; }
        public float? varianceQty { get; set; }
    }
}
