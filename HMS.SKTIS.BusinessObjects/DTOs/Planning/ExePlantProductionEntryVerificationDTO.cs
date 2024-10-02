using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class ExePlantProductionEntryVerificationDTO
    {
        public string ProductionEntryCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string ProcessGroup { get; set; }

        public int ProcessOrder { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public DateTime ProductionDate { get; set; }
        public float? TPKValue { get; set; }
        List<ExePlantProductionEntry> ExePlantProductionEntries { get; set; }
        public ExeProductionEntryReleaseDTO ExeProductionEntryRelease { get; set; }
    }
}
