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
    
    public partial class MntcRepairItemUsageView
    {
        public System.DateTime TransactionDate { get; set; }
        public string LocationCode { get; set; }
        public string ItemCodeSource { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public Nullable<int> QtyConvert { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}