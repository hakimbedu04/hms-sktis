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
    
    public partial class MaintenanceItemConversionDestinationView
    {
        public string ItemCodeSource { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemCodeDestinationDescription { get; set; }
        public Nullable<int> QtyConvert { get; set; }
        public Nullable<int> SourceStock { get; set; }
        public string LocationCode { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> QtyGood { get; set; }
        public Nullable<int> QtyDisposal { get; set; }
    }
}
