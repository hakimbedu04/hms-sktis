using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportProductionStockDTO
    {
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public double BeginStockInternalMove { get; set; }
        public double BeginStockExternalMove { get; set; }
        public double Production { get; set; }
        public double Planning { get; set; }
        public double VarianceStick { get; set; }
        public double VariancePercent { get; set; }
        public double PAP { get; set; }
        public double PAG { get; set; }
        public double EndingStockInternalMove { get; set; }
        public double EndingStockExternalMove { get; set; }
    }
}
