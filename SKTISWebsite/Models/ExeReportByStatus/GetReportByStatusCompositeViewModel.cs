using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeReportByStatus
{
    public class GetReportByStatusCompositeViewModel
    {
        public GetReportByStatusCompositeViewModel() {
            listProcess = new List<GetReportByStatusViewModel>();
        }

        public string ProcessGroup { get; set; }
        public string TotalActual { get; set; }
        public string TotalAbsen { get; set; }
        public string TotalWorkHourPerDay { get; set; }
        public string TotalProductionStick { get; set; }
        public string TotalStickHourPeople { get; set; }
        public string TotalStickHour { get; set; }
        public string TotalBalanceIndex { get; set; }
        public string TotalWorkHour { get; set; }
        public List<GetReportByStatusViewModel> listProcess { get; set; }
    }
}