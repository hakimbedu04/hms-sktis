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
    
    public partial class ExeProductionEntryReleaseTransactLogsView
    {
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string ProcessGroup { get; set; }
        public System.DateTime EblekDate { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public Nullable<bool> IsLocked { get; set; }
        public string Remark { get; set; }
        public string TransactionCode { get; set; }
        public Nullable<int> IDFlow { get; set; }
        public System.DateTime LogCreatedDate { get; set; }
        public System.DateTime LogUpdatedDate { get; set; }
    }
}
