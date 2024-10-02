using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantWorkerAbsenteeismDTO
    {
        public System.DateTime StartDateAbsent { get; set; }
        public System.DateTime OldValueStartDateAbsent { get; set; }
        public string EmployeeID { get; set; }
        public string OldValueEmployeeID { get; set; }
        public string AbsentType { get; set; }
        public System.DateTime EndDateAbsent { get; set; }
        public System.DateTime OldValueEndDateAbsent { get; set; }
        public string SktAbsentCode { get; set; }
        public string PayrollAbsentCode { get; set; }
        public string ePaf { get; set; }
        public string Attachment { get; set; }
        public string AttachmentPath { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string EmployeeNumber { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string GroupCode { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int Shift { get; set; }
        public int OldValueShift { get; set; }
        public bool IsFromWorkerAssignment { get; set; }
    }
}
