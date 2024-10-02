using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportProdStockPerBrandDTO
    {
        public string BrandGroupCode { get; set; }
        public string BrandGroup { get; set; }
        public int CountBrandGroup { get; set; }

        public IEnumerable<ExeReportProductionStockDTO> ListReportProdStock { get; set; }

        public double TotalBeginStockInMovePerBrand { get; set; }
        public double TotalBeginStockExtMovePerBrand { get; set; }
        public double TotalProdPerBrand { get; set; }
        public double TotalPAPPerBrand { get; set; }
        public double TotalPAGPerBrand { get; set; }
        public double TotalPlanningPerBrand { get; set; }
        public double TotalEndStockInMovePerBrand { get; set; }
        public double TotalEndStockExtMovePerBrand { get; set; }
        public double TotalVarStickPerBrand { get; set; }
        public double TotalVarStickPercentPerBrand { get; set; }
    }
}
