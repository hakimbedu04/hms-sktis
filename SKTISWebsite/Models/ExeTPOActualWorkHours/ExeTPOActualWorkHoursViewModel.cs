using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeTPOActualWorkHours
{
    public class ExeTPOActualWorkHoursViewModel : ViewModelBase
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string BrandCode { get; set; }
        public string StatusIdentifier { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string StatusEmp { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string BreakTime { get; set; }
    }
}