using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MntcRepairItemUsageDTO
    {
        public DateTime TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCodeSource { get; set; }
        public string ItemCodeDestination { get; set; }
        public int? QtyUsage { get; set; }

        public int? LastQtyUsage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
