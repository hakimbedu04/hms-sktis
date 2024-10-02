using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantProductionEntryInput : BaseInput
    {
        public string LocationCode { get; set; }

        public string LocationCompat { get; set; }
        public string UnitCode { get; set; }
        public string ProcessGroup { get; set; }
        public string Group { get; set; }
        public string Brand { get; set; }
        //public string TPKPlant { get; set; }
        //public string Target { get; set; }
        //public string Actual { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string AbsenType { get; set; }
        //public string FilterType { get; set; }
    }
}
