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
    
    public partial class MstGenProcess
    {
        public MstGenProcess()
        {
            this.ExeActualWorkHours = new HashSet<ExeActualWorkHour>();
            this.ExeTPOActualWorkHours = new HashSet<ExeTPOActualWorkHour>();
            this.MstTPOProductionGroups = new HashSet<MstTPOProductionGroup>();
            this.ProductAdjustments = new HashSet<ProductAdjustment>();
            this.MstGenProcessSettings = new HashSet<MstGenProcessSetting>();
            this.ExeReportByProcesses = new HashSet<ExeReportByProcess>();
            this.ExeReportByGroupsWeeklies = new HashSet<ExeReportByGroupsWeekly>();
            this.ExeReportByGroupsMonthlies = new HashSet<ExeReportByGroupsMonthly>();
            this.MstPlantProductionGroups = new HashSet<MstPlantProductionGroup>();
            this.ExeReportByGroups = new HashSet<ExeReportByGroup>();
        }
    
        public string ProcessGroup { get; set; }
        public string ProcessIdentifier { get; set; }
        public int ProcessOrder { get; set; }
        public Nullable<bool> StatusActive { get; set; }
        public bool WIP { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<ExeActualWorkHour> ExeActualWorkHours { get; set; }
        public virtual ICollection<ExeTPOActualWorkHour> ExeTPOActualWorkHours { get; set; }
        public virtual ICollection<MstTPOProductionGroup> MstTPOProductionGroups { get; set; }
        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; }
        public virtual ICollection<MstGenProcessSetting> MstGenProcessSettings { get; set; }
        public virtual ICollection<ExeReportByProcess> ExeReportByProcesses { get; set; }
        public virtual ICollection<ExeReportByGroupsWeekly> ExeReportByGroupsWeeklies { get; set; }
        public virtual ICollection<ExeReportByGroupsMonthly> ExeReportByGroupsMonthlies { get; set; }
        public virtual ICollection<MstPlantProductionGroup> MstPlantProductionGroups { get; set; }
        public virtual ICollection<ExeReportByGroup> ExeReportByGroups { get; set; }
    }
}
