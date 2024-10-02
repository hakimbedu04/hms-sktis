using System;

namespace SKTISWebsite.Models.MstPlantEmployeeJobsData
{
    public class MstEmployeeJobsDataViewModel : ViewModelBase
    {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string JoinDate { get; set; }
        public string Title_id { get; set; }
        public string ProcessSettingsCode { get; set; }
        public string Status { get; set; }
        public string CCT { get; set; }
        public string CCTDescription { get; set; }
        public string HCC { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public string Loc_id { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}