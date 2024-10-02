using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetUserAndEmailInput
    {
        public string LocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string Process { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public DateTime Date { get; set; }
        public string FunctionName { get; set; }
        public string ButtonName { get; set; }
        public string UserName { get; set; }
        public string EmailSubject { get; set; }
        public string Recipient { get; set; }
        public string Remark { get; set; }
        public string BrandCode { get; set; }
        public string FunctionNameDestination { get; set; }
        public string StatusEmp { get; set; }
        public int IDResponsibility { get; set; }
        public string Regional { get; set; }
        public string GroupCode { get; set; }

        public DateTime RequestDate { get; set; }
        public string RequestNumber { get; set; }

        public string NextGup { get; set; }

        public string ProductionEntryCode { get; set; }
        public int IDFlow { get; set; }

    }
}
