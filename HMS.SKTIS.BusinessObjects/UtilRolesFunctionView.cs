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
    
    public partial class UtilRolesFunctionView
    {
        public int IDFunction { get; set; }
        public Nullable<int> ParentIDFunction { get; set; }
        public string FunctionName { get; set; }
        public string FunctionType { get; set; }
        public string FunctionDescription { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int IDRole { get; set; }
    }
}