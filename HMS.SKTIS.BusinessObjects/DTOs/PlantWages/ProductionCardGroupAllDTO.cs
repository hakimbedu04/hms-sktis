using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class ProductionCardGroupAllDTO 
    {
        public int RevisionType { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string[] GroupCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameOnly { get; set; }
        public float? Production { get; set; }
        public DateTime ProductionDate { get; set; }
        public int? WorkHours { get; set; }
        public string Absent { get; set; }
        public float? UpahLain { get; set; }
        public string EblekAbsentType { get; set; }
        public string Remark { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ProductionCardCode { get; set; }
    }
}
