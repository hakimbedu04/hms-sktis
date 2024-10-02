using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportByStatusMonthlyDTO
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public int? ActualWorker { get; set; }
        public double? ActualAbsWorker { get; set; }
        public int? ActualWorkHour { get; set; }
        public int? PrdPerStk { get; set; }
        public int? StkPerHrPerPpl { get; set; }
        public int? StkPerHr { get; set; }
        public decimal? BalanceIndex { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
