using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetRepairItemUsageInput : BaseInput
    {
        public DateTime? TransactionDate { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemCodeSource { get; set; }
    }
}
