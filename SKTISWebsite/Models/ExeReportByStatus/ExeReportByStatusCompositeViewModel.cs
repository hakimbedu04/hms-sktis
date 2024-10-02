using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportByStatus
{
    public class ExeReportByStatusCompositeViewModel<T1>
    {
        public ExeReportByStatusCompositeViewModel()
        {
            StatusPerProcess = new List<T1>();
        }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        //public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        
        public string TotalBalanceIndexPerProcess { get; set; }
        public string TotalActualWorkHourPerProcess { get; set; }
        public string TotalWorkHour { get; set; }
        public string TotalProductionPerProcess { get; set; }
        public string TotalStickHourPerProcess { get; set; }
       
        public List<T1> StatusPerProcess { get; set; }
    }
}