using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class WagesReportAvailablePositionNumberInput : BaseInput
    {        
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string ProcessGroup { get; set; }
        public string Status { get; set; }
        public string GroupCode { get; set; }
    }
}
