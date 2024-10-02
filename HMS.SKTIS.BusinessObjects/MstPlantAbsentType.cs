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
    
    public partial class MstPlantAbsentType
    {
        public MstPlantAbsentType()
        {
            this.ExePlantProductionEntries = new HashSet<ExePlantProductionEntry>();
            this.ExePlantWorkerAbsenteeism = new HashSet<ExePlantWorkerAbsenteeism>();
        }
    
        public string AbsentType { get; set; }
        public string SktAbsentCode { get; set; }
        public string PayrollAbsentCode { get; set; }
        public string AlphaReplace { get; set; }
        public int MaxDay { get; set; }
        public Nullable<bool> ActiveInAbsent { get; set; }
        public Nullable<bool> ActiveInProductionEntry { get; set; }
        public string Remark { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Calculation { get; set; }
    
        public virtual ICollection<ExePlantProductionEntry> ExePlantProductionEntries { get; set; }
        public virtual ICollection<ExePlantWorkerAbsenteeism> ExePlantWorkerAbsenteeism { get; set; }
    }
}
