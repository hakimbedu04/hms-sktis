using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMaintenanceEquipmentItemConvertInput : BaseInput
    {
        public bool ConversionType { get; set; }
        public int KpsYear { get; set; }
       // public int KpsMonth { get; set; }
        public int KpsWeek { get; set; }
        public string LocationCode { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
