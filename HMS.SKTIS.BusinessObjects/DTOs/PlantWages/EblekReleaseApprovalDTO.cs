using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class EblekReleaseApprovalDTO
    {
        public string ProductionEntryCode { get; set; }
        public bool IsLocked { get; set; }
        public DateTime EblekDate { get; set; }
        public string BrandCode { get; set; }
        public string GroupCode { get; set; }
        public string Remark { get; set; }
        public string OldValueRemark { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public int? IDFlow { get; set; }
        public string ProcessGroup { get; set; }
    }
}
