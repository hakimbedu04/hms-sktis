using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.EquipmentRepairPlant
{
    public class InitEquipmentRepairPlantViewModel
    {
        public InitEquipmentRepairPlantViewModel()
        {
            DefaultTransactionDate = DateTime.Now.Date;
        }
        public DateTime DefaultTransactionDate { get; set; }

        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }

        public string DefaultLocation { get; set; }

        public string DefaultItemCode { get; set; }
    }
}
