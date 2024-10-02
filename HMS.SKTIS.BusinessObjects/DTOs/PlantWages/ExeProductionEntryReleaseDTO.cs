using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class ExeProductionEntryReleaseDTO
    {
        public string ProductionEntryCode { get; set; }
        public string Remark { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public ExePlantProductionEntryVerificationDTO ExePlantProductionEntryVerification { get; set; }
    }
}
