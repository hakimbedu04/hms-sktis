using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentTransferCompositeDTO
    {
        public DateTime TransferDate { get; set; }
        public string LocationCodeSource { get; set; }
        public string LocationCodeDestination { get; set; }
        public string ItemCode { get; set; }
        public int? QtyTransfer { get; set; }
        public string TransferNote { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string UnitCodeDestination { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}
