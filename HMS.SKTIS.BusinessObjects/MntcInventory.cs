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
    
    public partial class MntcInventory
    {
        public System.DateTime InventoryDate { get; set; }
        public string ItemStatus { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public Nullable<int> BeginningStock { get; set; }
        public Nullable<int> StockIn { get; set; }
        public Nullable<int> StockOut { get; set; }
        public Nullable<int> EndingStock { get; set; }
        public string UnitCode { get; set; }
    
        public virtual MstMntcItemLocation MstMntcItemLocation { get; set; }
    }
}
