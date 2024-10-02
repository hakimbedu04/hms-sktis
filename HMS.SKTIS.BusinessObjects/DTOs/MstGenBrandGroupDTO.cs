using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenBrandGroupDTO
    {
        public string BrandGroupCode { get; set; }
        public string BrandFamily { get; set; }
        public string PackType { get; set; }
        public string ClassType { get; set; }
        public int? StickPerPack { get; set; }
        public int? PackPerSlof { get; set; }
        public int? SlofPerBal { get; set; }
        public int? BalPerBox { get; set; }
        public string SKTBrandCode { get; set; }
        public string Description { get; set; }
        public float? CigarreteWeight { get; set; }
        public int? StickPerSlof { get; set; }
        public int? StickPerBal { get; set; }
        public int? StickPerBox { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int EmpPackage { get; set; }
    }
}
