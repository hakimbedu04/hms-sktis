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
    
    public partial class GetReportByStatus_Result
    {
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public Nullable<int> ActualWorker { get; set; }
        public Nullable<int> ActualAbsWorker { get; set; }
        public Nullable<decimal> ActualWorkHourPerDay { get; set; }
        public Nullable<long> ProductionStick { get; set; }
        public Nullable<decimal> StickHourPeople { get; set; }
        public Nullable<decimal> StickHour { get; set; }
    }
}
