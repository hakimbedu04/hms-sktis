using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentRequest
{
    public class EquipmentRequestViewModel : ViewModelBase
    {
        public string RequestDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        [DisplayFormat(NullDisplayText = "0")]
        public int? ReadyToUse { get; set; }
        [DisplayFormat(NullDisplayText = "0")]
        public int? OnUsed { get; set; }
        [DisplayFormat(NullDisplayText = "0")]
        public int? OnRepair { get; set; }
        public decimal TotalQuantity { get; set; }
        [DisplayFormat(NullDisplayText = "0")]
        public decimal? ApprovedQty { get; set; }
        public string LocationCode { get; set; }
        public string RequestNumber { get; set; }
    }
}