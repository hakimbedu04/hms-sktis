using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstTPOProductionGroupDTO
    {
        public string ProdGroup { get; set; }
        public string ProcessGroup { get; set; }
        public string LocationCode { get; set; }
        public string StatusEmp { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? WorkerCount { get; set; }
        public string ProdGroupComputed { get; set; }
    }
}
