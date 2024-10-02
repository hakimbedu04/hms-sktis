using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKTISWebsite.Models.ExePlantActualWorkHours
{
    public class ExePlantActualWorkHoursViewModel : ViewModelBase
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public string BrandCode { get; set; }
        public string StatusIdentifier { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? Shift { get; set; }
        public string StatusEmp { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string BreakTime { get; set; }
    }
}
