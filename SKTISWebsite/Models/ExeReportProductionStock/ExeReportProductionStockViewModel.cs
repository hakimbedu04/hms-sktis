using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportProductionStock
{
    public class ExeReportProductionStockViewModel
    {
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public string BeginStockInternalMove { get; set; }
        public string BeginStockExternalMove { get; set; }
        public string Production { get; set; }
        public string Planning { get; set; }
        public string VarianceStick { get; set; }
        public string VariancePercent { get; set; }
        public string PAP { get; set; }
        public string PAG { get; set; }
        public string EndingStockInternalMove { get; set; }
        public string EndingStockExternalMove { get; set; }
    }
}