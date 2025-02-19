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
    
    public partial class MstPlantUnit
    {
        public MstPlantUnit()
        {
            this.ExeActualWorkHours = new HashSet<ExeActualWorkHour>();
            this.ExeTPOActualWorkHours = new HashSet<ExeTPOActualWorkHour>();
            this.MntcEquipmentMovements = new HashSet<MntcEquipmentMovement>();
            this.MntcEquipmentRepairs = new HashSet<MntcEquipmentRepair>();
            this.ExePlantCKSubmits = new HashSet<ExePlantCKSubmit>();
            this.PlanPlantIndividualCapacityReferences = new HashSet<PlanPlantIndividualCapacityReference>();
            this.ProductAdjustments = new HashSet<ProductAdjustment>();
            this.ExeReportByProcesses = new HashSet<ExeReportByProcess>();
            this.ExeReportByGroupsWeeklies = new HashSet<ExeReportByGroupsWeekly>();
            this.ExeReportByGroupsMonthlies = new HashSet<ExeReportByGroupsMonthly>();
            this.MntcInventoryAdjustments = new HashSet<MntcInventoryAdjustment>();
            this.MstPlantProductionGroups = new HashSet<MstPlantProductionGroup>();
            this.ExeReportByGroups = new HashSet<ExeReportByGroup>();
        }
    
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public Nullable<bool> StatusActive { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual ICollection<ExeActualWorkHour> ExeActualWorkHours { get; set; }
        public virtual ICollection<ExeTPOActualWorkHour> ExeTPOActualWorkHours { get; set; }
        public virtual ICollection<MntcEquipmentMovement> MntcEquipmentMovements { get; set; }
        public virtual ICollection<MntcEquipmentRepair> MntcEquipmentRepairs { get; set; }
        public virtual MstGenLocation MstGenLocation { get; set; }
        public virtual ICollection<ExePlantCKSubmit> ExePlantCKSubmits { get; set; }
        public virtual ICollection<PlanPlantIndividualCapacityReference> PlanPlantIndividualCapacityReferences { get; set; }
        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; }
        public virtual ICollection<ExeReportByProcess> ExeReportByProcesses { get; set; }
        public virtual ICollection<ExeReportByGroupsWeekly> ExeReportByGroupsWeeklies { get; set; }
        public virtual ICollection<ExeReportByGroupsMonthly> ExeReportByGroupsMonthlies { get; set; }
        public virtual ICollection<MntcInventoryAdjustment> MntcInventoryAdjustments { get; set; }
        public virtual ICollection<MstPlantProductionGroup> MstPlantProductionGroups { get; set; }
        public virtual ICollection<ExeReportByGroup> ExeReportByGroups { get; set; }
    }
}
