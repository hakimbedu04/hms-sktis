using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstMntcConvertDTO
    {
        public string ItemCodeSource { get; set; }
        public string ItemCodeSourceDescription { get; set; }
        public string ItemCodeDestination { get; set; }
        public string ItemCodeDestinationDescription { get; set; }
        public bool? ConversionType { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? QtyConvert { get; set; }
        public List<MstMntcConvertCompositeDTO> ItemDestinationEquipment { get; set; }
    }
}
