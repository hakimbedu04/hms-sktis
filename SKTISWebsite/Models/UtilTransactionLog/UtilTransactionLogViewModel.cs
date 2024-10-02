using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.UtilTransactionLog
{
    public class UtilTransactionLogViewModel
    {
        public string TransactionCode { get; set; }
        public string TransactionDate { get; set; }
        public int? IDFlow { get; set; }
        public string Comments { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}