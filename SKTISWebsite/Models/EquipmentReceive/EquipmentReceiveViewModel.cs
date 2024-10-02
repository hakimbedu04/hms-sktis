namespace SKTISWebsite.Models.EquipmentReceive
{
    public class EquipmentReceiveViewModel : ViewModelBase
    {
        public string TransferDate { get; set; }
        public string ReceiveDate { get; set; }
        public string UnitCodeDestination { get; set; }
        public string LocationCodeSource { get; set; }
        public string LocationCodeDestination { get; set; }
        public string ItemCode { get; set; }
        public int? QtyTransfer { get; set; }
        public int? QtyReceive { get; set; }
        public string ReceiveNote { get; set; }
        public string DeliveryNote { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string UpdatedDate { get; set; }
    }
}
