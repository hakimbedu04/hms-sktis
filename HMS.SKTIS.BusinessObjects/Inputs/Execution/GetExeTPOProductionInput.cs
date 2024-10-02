using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExeTPOProductionInput :BaseInput
    {
        public GetExeTPOProductionInput()
        {
            ProductionGroup = null;
        }

        public string LocationCode { get; set; }
        public string Process { get; set; }
        public string Status { get; set; }
        public string Brand {get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime? Date { get; set; }
        public string ProductionGroup { get; set; }
    }
}
