using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetPlantEquipmentRepairsTPOInput : BaseInput
    {
        public string LocationCode { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
