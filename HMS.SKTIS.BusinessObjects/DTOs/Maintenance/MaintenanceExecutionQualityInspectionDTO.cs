using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class MaintenanceExecutionQualityInspectionDTO
    {
        public DateTime TransactionDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string PurchaseNumber { get; set; }
        public string RequestNumber { get; set; }
        public string DeliveryNote { get; set; }
        public string Comments { get; set; }
        public string Supplier { get; set; }
        public string PreviousOutstanding { get; set; }
        public int? QTYTransit { get; set; }
        public int? QtyReceiving { get; set; }
        public int? QtyPass { get; set; }
        public int? QtyReject { get; set; }
        public int? QtyOutstanding { get; set; }
        public int? QtyReturn { get; set; }
        public int? Shift { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MntcQICode { get; set; }
        public decimal? QtyLeftOver { get; set; }
        public decimal? QtyLeftOverCount { get; set; }
    }
}
