using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantActualWorkHoursInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public string StatusEmp { get; set; }
        public DateTime? Date { get; set; }
        public int Shift { get; set; }
        public string Brand { get; set; }
    }
}
