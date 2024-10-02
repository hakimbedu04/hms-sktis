using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstMntcConvertCompositeDTO
    {
        public string ItemCodeSource { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? QtyConvert { get; set; }
    }
}
