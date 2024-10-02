using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ExeReportByProcess
{
    public class ExeReportByProcessViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProcessGroup { get; set; }
        public int ProcessOrder { get; set; }
        public int Shift { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public int UOMOrder { get; set; }
        public double Production { get; set; }
        public double KeluarBersih { get; set; }
        public double RejectSample { get; set; }
        public double BeginningStock { get; set; }
        public double EndingStock { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
