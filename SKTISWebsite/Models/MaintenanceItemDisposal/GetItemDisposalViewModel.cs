using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MaintenanceItemDisposal
{
    public class GetItemDisposalViewModel : ViewModelBase
    {
        public DateTime TransactionDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public int QtyDisposal { get; set; }
        public int? Shift { get; set; }
        public string ItemDescription { get; set; }
        public int? EndingStock { get; set; }
        public int? EndingStockPastDate { get; set; }
        public int? CalculatedEndingStock { get; set; }
        public string UpdatedDate { get; set; }
    }
}