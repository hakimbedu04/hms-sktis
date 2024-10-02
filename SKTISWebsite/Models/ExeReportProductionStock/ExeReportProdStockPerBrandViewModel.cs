using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportProductionStock
{
    public class ExeReportProdStockPerBrandViewModel
    {
        public string BrandGroupCode { get; set; }
        public string BrandGroup { get; set; }
        public int CountBrandGroup { get; set; }

        public IEnumerable<ExeReportProductionStockViewModel> ListReportProdStock { get; set; }

        public string TotalBeginStockInMovePerBrand { get; set; }
        public string TotalBeginStockExtMovePerBrand { get; set; }
        public string TotalProdPerBrand { get; set; }
        public string TotalPAPPerBrand { get; set; }
        public string TotalPAGPerBrand { get; set; }
        public string TotalPlanningPerBrand { get; set; }
        public string TotalEndStockInMovePerBrand { get; set; }
        public string TotalEndStockExtMovePerBrand { get; set; }
        public string TotalVarStickPerBrand { get; set; }
        public string TotalVarStickPercentPerBrand { get; set; }
    }
}