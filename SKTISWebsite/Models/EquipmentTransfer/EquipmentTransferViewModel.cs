using System;

namespace SKTISWebsite.Models.EquipmentTransfer
{
    public class EquipmentTransferViewModel : ViewModelBase
    {
        public string TransferDate { get; set; }
        public string LocationCodeSource { get; set; }
        public string LocationCodeDestination { get; set; }
        public string ItemCode { get; set; }
        public int? Quantity { get; set; }
        public int? Stock { get; set; }
        public string DeliveryNote { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string UnitCodeDestination { get; set; }
        public string UpdatedDate { get; set; }
    }
}
