using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MntcRequestToLocationDTO
    {
        public System.DateTime FulFillmentDate { get; set; }
        public string RequestNumber { get; set; }
        public string LocationCode { get; set; }
        public int QtyFromLocation { get; set; }
        public string ItemCode { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
