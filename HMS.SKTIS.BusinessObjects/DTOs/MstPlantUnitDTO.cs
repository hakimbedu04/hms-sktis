using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstPlantUnitDTO
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitName { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    
    }
}
