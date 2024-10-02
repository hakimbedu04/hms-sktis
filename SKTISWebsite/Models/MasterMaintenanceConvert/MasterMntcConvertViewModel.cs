using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Models.MasterMaintenanceConvert
{
    public class MasterMntcConvertViewModel : ViewModelBase
    {
        public string ItemCodeSource { get; set; }
        public string ItemCodeSourceDescription { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemCodeDestinationDescription { get; set; }
        public bool? ConversionType { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? QtyConvert { get; set; }
        public List<MstMntcConvertCompositeDTO> ItemDestinationEquipment { get; set; }
    }
}