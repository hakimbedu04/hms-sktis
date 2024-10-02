using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstPlantAbsentTypeDTO
    {
        public string AbsentType { get; set; }
        public string SktAbsentCode { get; set; }
        public string PayrollAbsentCode { get; set; }
        public string AlphaReplace { get; set; }
        public int? MaxDay { get; set; }
        public bool? ActiveInAbsent { get; set; }
        public bool? ActiveInProductionEntry { get; set; }
        public string Calculation { get; set; }
        public string Remark { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string OldAbsentType { get; set; }
    }
}
