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
    
    public partial class InventoryByStatusView
    {
        public long RowID { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public Nullable<int> ReadyToUse { get; set; }
        public Nullable<int> OnUse { get; set; }
        public Nullable<int> OnRepair { get; set; }
    }
}