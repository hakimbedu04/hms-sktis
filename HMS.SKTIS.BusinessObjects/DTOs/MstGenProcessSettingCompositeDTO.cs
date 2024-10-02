using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
   public class MstGenProcessSettingCompositeDTO
    {
        public string ProcessGroup { get; set; }
        public string BrandGroupCode { get; set; }
        public int StdStickPerHour { get; set; }
    }
}
