using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class WorkerBrandAssigmentDTO
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int Shift { get; set; }
        public DateTime TPKPlantStartProductionDate { get; set; }
        public string EmployeeNumber { get; set; }
        public bool Status { get; set; }
        public string EmployeeID { get; set; }
    }

}
