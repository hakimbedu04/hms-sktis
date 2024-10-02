using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilTransactionLogDTO
    {
        public string TransactionCode { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int? IDFlow { get; set; }
        public string Comments { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public UtilFlow UtilFlow { get; set; }
    }
}
