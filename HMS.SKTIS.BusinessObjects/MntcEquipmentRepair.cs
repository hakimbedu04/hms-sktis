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
    
    public partial class MntcEquipmentRepair
    {
        public System.DateTime TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public Nullable<int> Shift { get; set; }
        public string PreviousOutstanding { get; set; }
        public Nullable<int> QtyRepairRequest { get; set; }
        public Nullable<int> QtyCompletion { get; set; }
        public Nullable<int> QtyOutstanding { get; set; }
        public Nullable<int> QtyBadStock { get; set; }
        public Nullable<int> QtyTakenByUnit { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual MstMntcItem MstMntcItem { get; set; }
        public virtual MstPlantUnit MstPlantUnit { get; set; }
    }
}
