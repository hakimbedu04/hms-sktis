using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstProcessBrandViewDTO
    {
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
    }
}
