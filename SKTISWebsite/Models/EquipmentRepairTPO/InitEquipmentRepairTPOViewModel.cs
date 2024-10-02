using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentRepairTPO
{
    public class InitEquipmentRepairTPOViewModel
    {
        public InitEquipmentRepairTPOViewModel()
        {
            DefaultTransactionDate = DateTime.Now.Date;
        }
        public DateTime DefaultTransactionDate { get; set; }

        public List<LocationLookupList> TPOChildLocationLookupList { get; set; }

        public string DefaultLocation { get; set; }
    }
}
