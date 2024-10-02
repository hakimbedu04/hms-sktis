using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExePlantWorkerAbsenteeism
{
    public class ExePlantWorkerAbsenteeismViewModel : ViewModelBase
    {
        public string StartDateAbsent { get; set; }
        public string OldValueStartDateAbsent { get; set; }
        public string EmployeeID { get; set; }
        public string OldValueEmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string ProcessSettingsCode { get; set; }        
        public string AbsentType { get; set; }
        public string EndDateAbsent { get; set; }
        public string OldValueEndDateAbsent { get; set; }
        public string SktAbsentCode { get; set; }
        public string PayrollAbsentCode { get; set; }
        public string ePaf { get; set; }
        public string Attachment { get; set; }
        public string AttachmentPath { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string EmployeeNumber { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string GroupCode { get; set; }
        public string TransactionDate { get; set; }
        public int Shift { get; set; }
        public int OldValueShift { get; set; }
        public string AttachmentName {
            get
            {
                return string.IsNullOrEmpty(AttachmentPath) ? null : Path.GetFileName(AttachmentPath);
            }
        }
        public bool IsFromEblek { get; set; }
        public string OldValueAbsentType { get; set; }
        public string StateOnEdit { get; set; }
        public string EblekStatus { get; set; }
    }
}