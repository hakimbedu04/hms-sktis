using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstTPOInfoDTO
    {
        public string LocationCode { get; set; }
        public string TPORank { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public string BankType { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BankBranch { get; set; }
        public string Owner { get; set; }
        public DateTime Established { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string LocationName { get; set; }
    }
}
