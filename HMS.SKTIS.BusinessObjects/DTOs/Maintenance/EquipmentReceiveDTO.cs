using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentReceiveDTO
    {
        public DateTime TransferDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string LocationCodeSource { get; set; }
        public string UnitCodeDestination { get; set; }
        public string LocationCodeDestination { get; set; }
        public string ItemCode { get; set; }
        public int? QtyTransfer { get; set; }
        public int? QtyReceive { get; set; }
        public string TransferNote { get; set; }
        public string ReceiveNote { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
