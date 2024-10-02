using System.Collections.Generic;
namespace SKTISWebsite.Models.EquipmentRepairPlant
{
    public class EquipmentRepairViewModel : ViewModelBase
    {
        public string TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public string PreviousOutstanding { get; set; }
        public int? QtyRepairRequest { get; set; }
        public int? QtyCompletion { get; set; }
        public int? QtyOutstanding { get; set; }
        public int? QtyBadStock { get; set; }
        public int? QtyTakenByUnit { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public List<SparepartViewModel> Sparepart { get; set; }
    }
}
