using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenBrandPackageItemDTO
    {
        public string BrandGroupCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int? Qty { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

    }
}
