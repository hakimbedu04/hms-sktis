using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.EquipmentRepairPlant
{
    public class InsertUpdateRepairPlant<T> where T : class    
    {
        public IDictionary<string, string> Parameters { get; set; }
        public List<T> New { get; set; }
        public List<T> Edit { get; set; }
        public List<SparepartViewModel> Sparepart { get; set; }
    }
}