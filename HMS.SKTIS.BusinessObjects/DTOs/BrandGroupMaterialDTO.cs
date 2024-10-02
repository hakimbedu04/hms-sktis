using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class BrandGroupMaterialDTO
    {
        public string MaterialCode { get; set; }
        public string OldMaterialCode { get; set; }
        //public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public string Uom { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
