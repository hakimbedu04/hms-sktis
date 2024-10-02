using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportProductionStock
{
    public class ExeReportProdStockPerBrandGroupCodeViewModel
    {
        public string BrandGroupCode { get; set; }
        public int CountBrandGroupCode { get; set; }

        public IEnumerable<ExeReportProdStockPerBrandViewModel> ListReportProdStockPerBrand { get; set; }

        public string TotalBeginStockInMovePerBrandGroupCode { get; set; }
        public string TotalBeginStockExtMovePerBrandGroupCode { get; set; }
        public string TotalProdPerBrandGroupCode { get; set; }
        public string TotalPAPPerBrandGroupCode { get; set; }
        public string TotalPAGPerBrandGroupCode { get; set; }
        public string TotalPlanningPerBrandGroupCode { get; set; }
        public string TotalEndStockInMovePerBrandGroupCode { get; set; }
        public string TotalEndStockExtMovePerBrandGroupCode { get; set; }
        public string TotalVarStickPerBrandGroupCode { get; set; }
        public string TotalVarStickPercentPerBrandGroupCode { get; set; }
    }
}