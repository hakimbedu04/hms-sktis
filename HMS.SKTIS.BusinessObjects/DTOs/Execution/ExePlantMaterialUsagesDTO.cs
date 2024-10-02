using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantMaterialUsagesDTO
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string BrandGroupCode { get; set; }
        public string MaterialCode { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public DateTime ProductionDate { get; set; }
        public float? Sisa { get; set; }
        public float? Ambil1 { get; set; }
        public float? Ambil2 { get; set; }
        public float? Ambil3 { get; set; }
        public float? TobFM { get; set; }
        public float? TobStem { get; set; }
        public float? TobSapon { get; set; }
        public float? UncountableWaste { get; set; }
        public float? CountableWaste { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public double? Production { get; set; }
    }
}
