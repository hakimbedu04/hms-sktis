using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstPlantProductionGroupCompositeDTO
    {
        public string GroupCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessSettingsCode { get; set; }
        public int? WorkerCount { get; set; }
        public string Leader1 { get; set; }
        public string Leader2 { get; set; }
        public string InspectionLeader { get; set; }
        public string NextGroupCode { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string LeaderInspectionName { get; set; }
        public string Leader1Name { get; set; }
        public string Leader2Name { get; set; }
    }
}
