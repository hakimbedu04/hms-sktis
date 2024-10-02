using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentRepairTPODTO
    {
        public DateTime TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public int? Shift { get; set; }
        public string PreviousOutstanding { get; set; }
        public int? QtyRepairRequest { get; set; }
        public int? QtyCompletion { get; set; }
        public int? QtyOutstanding { get; set; }
        public int? QtyBadStock { get; set; }
        public int? QtyTakenByUnit { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        //For MntcRepairItemUsage
        public string ItemCodeDestination { get; set; }
        public int? QtyUsage { get; set; }

        public int? LastQtyUsage { get; set; }
    }
}
