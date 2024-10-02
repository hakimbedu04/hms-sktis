using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.MasterTOPInfo
{
    public class MasterTPOInfoViewModels : ViewModelBase
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
        public string Established { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string LocationName { get; set; }
    }
}