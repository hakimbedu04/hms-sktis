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
    
    public partial class ExeReportDailyProductionAchievementView
    {
        public string LocationCode { get; set; }
        public string ABBR { get; set; }
        public string ParentLocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string SKTBrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public Nullable<int> Production { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public Nullable<double> TPKValue { get; set; }
        public Nullable<int> WorkerCount { get; set; }
        public float Package { get; set; }
        public Nullable<int> StdStickPerHour { get; set; }
        public string BrandCode { get; set; }
    }
}