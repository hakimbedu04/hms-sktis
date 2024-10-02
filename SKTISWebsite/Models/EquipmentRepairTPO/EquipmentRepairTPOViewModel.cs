using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.EquipmentRepairTPO
{
    public class EquipmentRepairTPOViewModel : ViewModelBase
    {
        public string TransactionDate { get; set; }
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
        public string UpdatedDate { get; set; }
        public List<ItemSelectionItemCodeDetail> ItemSelectionItemCodeDetails { get; set; }
    }
}
