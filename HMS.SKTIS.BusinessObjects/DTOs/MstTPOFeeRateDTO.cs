using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstTPOFeeRateDTO
    {
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime PreviousEffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public decimal JKN { get; set; }
        public decimal Jl1 { get; set; }
        public decimal Jl2 { get; set; }
        public decimal Jl3 { get; set; }
        public decimal Jl4 { get; set; }
        public decimal ManagementFee { get; set; }
        public decimal ProductivityIncentives { get; set; }
        public string MemoRef { get; set; }
        public string MemoFile { get; set; }
        public string MemoPath { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
