using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstEmployeeJobsDataActiveDTO
    {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string TitleId { get; set; }
        public string ProcessSettingsCode { get; set; }
        public string Status { get; set; }
        public string CCT { get; set; }
        public string CCTDescription { get; set; }
        public string HCC { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
