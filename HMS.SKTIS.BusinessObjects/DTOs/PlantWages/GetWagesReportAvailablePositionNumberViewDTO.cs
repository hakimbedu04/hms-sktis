using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class GetWagesReportAvailablePositionNumberViewDTO
    {
        public string LocationCode { get; set; }
        public string EmployeeNumber { get; set; }
        public string GroupCode { get; set; }
        public string Status { get; set; }
        public string UnitCode { get; set; }
        public string ProcessSettingsCode { get; set; }
    }
}
