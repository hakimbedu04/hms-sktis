using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MntcEquipmentItemDisposalCompositeDTO
    {
        public DateTime TransactionDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public int QtyDisposal { get; set; }
        public int? Shift { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        //MstMntcItem
        public string ItemDescription { get; set; }

        //MntcInventory
        public int? EndingStock { get; set; }
        public int? EndingStockPastDate { get; set; }
    }
}
