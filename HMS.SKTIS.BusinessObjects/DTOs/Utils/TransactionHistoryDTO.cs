using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class TransactionHistoryDTO
    {
        public string name { get; set; }
        public string role { get; set; }
        public string action { get; set; }
        public System.DateTime date { get; set; }
        public string note { get; set; }
    }
}
