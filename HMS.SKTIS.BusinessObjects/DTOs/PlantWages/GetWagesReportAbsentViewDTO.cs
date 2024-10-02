using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class GetWagesReportAbsentViewDTO
    {
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string LocationCode { get; set; }
        public string ProdUnit { get; set; }
        public string ProcessGroup { get; set; }
        public string ProdGroup { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<int> Terdaftar { get; set; }
        public Nullable<int> Alpa { get; set; }
        public Nullable<int> Ijin { get; set; }
        public Nullable<int> Sakit { get; set; }
        public Nullable<int> Cuti { get; set; }
        public Nullable<int> CutiHamil { get; set; }
        public Nullable<int> CutiTahunan { get; set; }
        public Nullable<int> Skorsing { get; set; }
        public Nullable<int> Keluar { get; set; }
        public Nullable<int> Masuk { get; set; }
        public int Turnover { get; set; }
        public Nullable<int> TurnoverPersen { get; set; }
        public Nullable<double> Kehadiran { get; set; }
        public Nullable<double> Absensi { get; set; }
    }
}
