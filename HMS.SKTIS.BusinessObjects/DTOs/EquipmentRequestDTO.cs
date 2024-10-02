using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class EquipmentRequestDTO
    {
        public DateTime FulFillmentDate { get; set; }
        public string LocationCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Qty { get; set; }
        public decimal? ApprovedQty { get; set; }
        public decimal? FullfillmentQty { get; set; }
        public decimal? OutstandingQty { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public decimal? QtyLeftOver { get; set; }
    }
}
