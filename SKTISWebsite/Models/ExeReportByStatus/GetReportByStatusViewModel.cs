using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportByStatus
{
    public class GetReportByStatusViewModel
    {
        public string ProcessGroup { get; set; }
        public string StatusEmp { get; set; }
        public string ActualWorker { get; set; }
        public string ActualAbsWorker { get; set; }
        public string ActualWorkHourPerDay { get; set; }
        public string ProductionStick { get; set; }
        public string StickHourPeople { get; set; }
        public string StickHour { get; set; }
    }
}