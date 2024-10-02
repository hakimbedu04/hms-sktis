using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportByStatus
{
    public class ExeReportByStatusMonthlyViewModel
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string BrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public int? ActualWorker { get; set; }
        public double? ActualAbsWorker { get; set; }
        public double? ActualWorkHour { get; set; }
        public int? PrdPerStk { get; set; }
        public double? StkPerHrPerPpl { get; set; }
        public double? StkPerHr { get; set; }
        public decimal? BalanceIndex { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}