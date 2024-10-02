using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class GetWagesReportAbsentGroupViewDTO
    {
        public Nullable<long> id { get; set; }
        public string LocationCode { get; set; }
        public string ProdUnit { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<int> Alpa { get; set; }
        public Nullable<int> Ijin { get; set; }
        public Nullable<int> Sakit { get; set; }
        public Nullable<int> Cuti { get; set; }
        public Nullable<int> CutiHamil { get; set; }
        public Nullable<int> CutiTahunan { get; set; }
        public Nullable<int> Skorsing { get; set; }
        public Nullable<int> HKNTotal { get; set; }
        public Nullable<int> AbsentTotal { get; set; }
        public Nullable<double> ProductionTotal { get; set; }
        public Nullable<int> JKTotal { get; set; }
        public Nullable<double> Productivity { get; set; }
    }
}
