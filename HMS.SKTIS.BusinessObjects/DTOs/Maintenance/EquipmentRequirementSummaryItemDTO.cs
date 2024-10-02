using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentRequirementSummaryItemDTO
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public List<int?> Quantity { get; set; }
    }
}
