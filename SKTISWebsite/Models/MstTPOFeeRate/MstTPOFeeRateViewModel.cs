using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MstTPOFeeRate
{
    public class MstTPOFeeRateViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string EffectiveDate { get; set; }
        public string PreviousEffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
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
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MemoFileName
        {
            get
            {
                var u = string.IsNullOrEmpty(MemoPath) ? null : Path.GetFileName(MemoPath);
                return u;
            }
        }
    }
}