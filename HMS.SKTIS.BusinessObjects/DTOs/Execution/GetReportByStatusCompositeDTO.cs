using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class GetReportByStatusCompositeDTO
    {
        public GetReportByStatusCompositeDTO() {
            listProcess = new List<GetReportByStatus_Result>();
        }

        public string ProcessGroup { get; set; }
        public int TotalActual { get; set; }
        public int TotalAbsen { get; set; }
        public decimal TotalWorkHourPerDay { get; set; }
        public long TotalProductionStick { get; set; }
        public decimal TotalStickHourPeople { get; set; }
        public decimal TotalStickHour { get; set; }
        public decimal TotalBalanceIndex { get; set; }
        public decimal TotalWorkHour { get; set; }
        public List<GetReportByStatus_Result> listProcess { get; set; }
    }
}
