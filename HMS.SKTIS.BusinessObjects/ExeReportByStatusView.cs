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
    
    public partial class ExeReportByStatusView
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public int Multi_CUTT { get; set; }
        public Nullable<int> ActualWorker { get; set; }
        public Nullable<double> ActualAbsWorker { get; set; }
        public Nullable<double> ActualWorkHour { get; set; }
        public Nullable<int> PrdPerStk { get; set; }
        public Nullable<double> StkPerHrPerPpl { get; set; }
        public Nullable<double> StkPerHr { get; set; }
        public Nullable<decimal> BalanceIndex { get; set; }
        public System.DateTime ProductionDate { get; set; }
        public string StatusIdentifier { get; set; }
    }
}