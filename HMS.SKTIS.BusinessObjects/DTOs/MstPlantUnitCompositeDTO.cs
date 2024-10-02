using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstPlantUnitCompositeDTO
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitName { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string PPC { get; set; }
        public string HRA { get; set; }
    }
}
