using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class EquipmentReceiveCompositeDTO
    {
        public DateTime TransferDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string LocationCodeSource { get; set; }
        public string UnitCodeDestination { get; set; }
        public string LocationCodeDestination { get; set; }
        public string ItemCode { get; set; }
        public int? QtyTransfer { get; set; }
        public int? QtyReceive { get; set; }
        public string ReceiveNote { get; set; }
        public string TransferNote { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
