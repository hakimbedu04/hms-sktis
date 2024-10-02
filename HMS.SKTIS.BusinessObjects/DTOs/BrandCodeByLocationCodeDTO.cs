using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class BrandCodeByLocationCodeDTO
    {
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string LocationCode { get; set; }
        public bool? StatusActive { get; set; }
        public string SKTBrandCode { get; set; }
    }
}
