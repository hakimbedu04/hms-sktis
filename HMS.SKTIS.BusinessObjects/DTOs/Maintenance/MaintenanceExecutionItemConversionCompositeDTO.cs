using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class MaintenanceExecutionItemConversionCompositeDTO
    {
        public string ItemCodeSource { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemCodeDestinationDescription { get; set; }
        public int? QtyConvert { get; set; }
        public int? SourceStock { get; set; }
        //public int? DestinationStock { get; set; }
        public int? QtyGood { get; set; }
        public int? QtyDisposal { get; set; }
    }
}
