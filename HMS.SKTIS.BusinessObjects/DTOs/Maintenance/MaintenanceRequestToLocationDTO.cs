using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Maintenance
{
    public class MaintenanceRequestToLocationDTO
    {
        public DateTime FulFillmentDate { get; set; }
        public string RequestNumber { get; set; }
        public string LocationCode { get; set; }
        public int QtyFromLocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
