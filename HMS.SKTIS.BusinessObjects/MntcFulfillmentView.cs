//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HMS.SKTIS.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    
    public partial class MntcFulfillmentView
    {
        public string LocationCode { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public string RequestNumber { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> FulFillmentDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<int> ReadyToUse { get; set; }
        public Nullable<int> OnUse { get; set; }
        public Nullable<int> OnRepair { get; set; }
        public decimal RequestedQuantity { get; set; }
        public Nullable<decimal> ApprovedQty { get; set; }
        public Nullable<decimal> RequestToQty { get; set; }
        public Nullable<decimal> PurchaseQuantity { get; set; }
        public string PurchaseNumber { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}
