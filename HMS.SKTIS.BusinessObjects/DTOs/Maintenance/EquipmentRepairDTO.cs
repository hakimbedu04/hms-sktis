using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentRepairDTO
    {
        public DateTime TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public string PreviousOutstanding { get; set; }
        public int? QtyRepairRequest { get; set; }
        public int? QtyCompletion { get; set; }
        public int? QtyOutstanding { get; set; }
        public int? QtyBadStock { get; set; }
        public int? QtyTakenByUnit { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
