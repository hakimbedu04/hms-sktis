using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;

namespace SKTISWebsite.Models.MaintenanceExecutionItemConversion
{
    public class MaintenanceExecutionItemConversionViewModel : ViewModelBase
    {
        public string TransactionDate { get; set; }
        public string LocationCode { get; set; }
        public string ItemCodeSource { get; set; }
        public string ItemCodeSourceDescription { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemCodeDestinationDescription { get; set; }
        public int SourceStock { get; set; }
        public int DestinationStock { get; set; }
        public int? QtyGood { get; set; }
        public int? QtyDisposal { get; set; }
        public bool ConversionType { get; set; }
        public int? Shift { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public List<MaintenanceExecutionItemConversionCompositeDTO> ItemDestinationEquipments { get; set; }
        public string SourceStatus { get; set; }
    }
}