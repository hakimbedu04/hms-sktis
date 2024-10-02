using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstTPOPackageDTO
    {
        public string TPOPackageCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public float? Package { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string MemoRef { get; set; }
        public string MemoFile { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MemoPath { get; set; }
        public string LocationName { get; set; }

        public DateTime EffectiveDateOld { get; set; }
    }
}
