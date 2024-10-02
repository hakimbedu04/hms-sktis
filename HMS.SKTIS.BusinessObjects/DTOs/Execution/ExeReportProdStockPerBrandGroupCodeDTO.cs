using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportProdStockPerBrandGroupCodeDTO
    {
        public string BrandGroupCode { get; set; }
        public int CountBrandGroupCode { get; set; }

        public IEnumerable<ExeReportProdStockPerBrandDTO> ListReportProdStockPerBrand { get; set; }

        public double TotalBeginStockInMovePerBrandGroupCode { get; set; }
        public double TotalBeginStockExtMovePerBrandGroupCode { get; set; }
        public double TotalProdPerBrandGroupCode { get; set; }
        public double TotalPAPPerBrandGroupCode { get; set; }
        public double TotalPAGPerBrandGroupCode { get; set; }
        public double TotalPlanningPerBrandGroupCode { get; set; }
        public double TotalEndStockInMovePerBrandGroupCode { get; set; }
        public double TotalEndStockExtMovePerBrandGroupCode { get; set; }
        public double TotalVarStickPerBrandGroupCode { get; set; }
        public double TotalVarStickPercentPerBrandGroupCode { get; set; }
    }
}
