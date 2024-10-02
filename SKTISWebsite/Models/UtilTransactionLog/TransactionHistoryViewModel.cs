using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.UtilTransactionLog
{
    public class TransactionHistoryViewModel
    {
        public string name { get; set; }
        public string role { get; set; }
        public string action { get; set; }
        public string date { get; set; }
        public string note { get; set; }
    }
}