using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MstTPOPackage
{
    public class MstTPOPackageViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public float? Package { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string MemoRef { get; set; }
        public string MemoFile { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string MemoPath { get; set; }
        public string LocationName { get; set; }
        public string MemoFileName
        {
            get
            {
                var u = string.IsNullOrEmpty(MemoPath) ? null : Path.GetFileName(MemoPath);
                return u;
            }
        }

        public string EffectiveDateOld { get; set; }
    }
}