using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetProductionCardApprovalListInput : BaseInput
    {
        public string FilterType { get; set; }
        public string TransactionStatus { get; set; }
        public bool IsMyAction { get; set; }
        public string CurrentUser { get; set; }
        public int MonthStart { get; set; }
        public int MonthEnd { get; set; }
        public int WeekStart { get; set; }
        public int WeekEnd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
