using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class PlanPlantGroupShiftDTO
    {
        public PlanPlantGroupShiftDTO()
        {
            Shift = 1;
        }
        public DateTime? StartDate { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public DateTime? EndDate { get; set; }
        public int Shift { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
