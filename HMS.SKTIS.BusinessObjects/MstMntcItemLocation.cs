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
    
    public partial class MstMntcItemLocation
    {
        public MstMntcItemLocation()
        {
            this.MntcEquipmentItemDisposals = new HashSet<MntcEquipmentItemDisposal>();
            this.MntcInventories = new HashSet<MntcInventory>();
            this.MntcEquipmentRequests = new HashSet<MntcEquipmentRequest>();
        }
    
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public int BufferStock { get; set; }
        public int MinOrder { get; set; }
        public Nullable<int> StockReadyToUse { get; set; }
        public Nullable<int> StockAll { get; set; }
        public string Remark { get; set; }
        public Nullable<int> AVGWeeklyUsage { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ItemType { get; set; }
    
        public virtual ICollection<MntcEquipmentItemDisposal> MntcEquipmentItemDisposals { get; set; }
        public virtual ICollection<MntcInventory> MntcInventories { get; set; }
        public virtual MstGenLocation MstGenLocation { get; set; }
        public virtual MstMntcItem MstMntcItem { get; set; }
        public virtual ICollection<MntcEquipmentRequest> MntcEquipmentRequests { get; set; }
    }
}
