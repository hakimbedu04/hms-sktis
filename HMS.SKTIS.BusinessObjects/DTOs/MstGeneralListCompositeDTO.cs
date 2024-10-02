using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGeneralListCompositeDTO
    {
        public string ListGroup { get; set; }
        public string ListDetail { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
