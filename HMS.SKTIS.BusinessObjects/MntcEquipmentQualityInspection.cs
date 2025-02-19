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
    
    public partial class MntcEquipmentQualityInspection
    {
        public System.DateTime TransactionDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string PurchaseNumber { get; set; }
        public string RequestNumber { get; set; }
        public string DeliveryNote { get; set; }
        public string Comments { get; set; }
        public string Supplier { get; set; }
        public string PreviousOutstanding { get; set; }
        public Nullable<int> QTYTransit { get; set; }
        public Nullable<int> QtyReceiving { get; set; }
        public Nullable<int> QtyPass { get; set; }
        public Nullable<int> QtyReject { get; set; }
        public Nullable<int> QtyOutstanding { get; set; }
        public Nullable<int> QtyReturn { get; set; }
        public Nullable<int> Shift { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MntcQICode { get; set; }
    
        public virtual MstGenLocation MstGenLocation { get; set; }
        public virtual MstMntcItem MstMntcItem { get; set; }
    }
}
