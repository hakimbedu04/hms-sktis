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
    
    public partial class MntcEquipmentRequest
    {
        public MntcEquipmentRequest()
        {
            this.MntcEquipmentFulfillments = new HashSet<MntcEquipmentFulfillment>();
        }
    
        public System.DateTime RequestDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string RequestNumber { get; set; }
        public decimal Qty { get; set; }
        public Nullable<decimal> ApprovedQty { get; set; }
        public Nullable<decimal> FullfillmentQty { get; set; }
        public Nullable<decimal> OutstandingQty { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<decimal> QtyLeftOver { get; set; }
    
        public virtual ICollection<MntcEquipmentFulfillment> MntcEquipmentFulfillments { get; set; }
        public virtual MstMntcItemLocation MstMntcItemLocation { get; set; }
    }
}
