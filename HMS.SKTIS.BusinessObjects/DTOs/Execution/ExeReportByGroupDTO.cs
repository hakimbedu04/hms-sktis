using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportByGroupDTO
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string StatusEmp { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int Shift { get; set; }
        public double Production { get; set; }
        public float TPKValue { get; set; }
        public double WorkHour { get; set; }
        public int WeekDay { get; set; }
        public double Absennce_A { get; set; }
        public double Absence_I { get; set; }
        public double Absence_C { get; set; }
        public double Absence_CH { get; set; }
        public double Absence_CT { get; set; }
        public double Absence_SLS { get; set; }
        public double Absence_SLP { get; set; }
        public double Absence_ETC { get; set; }
        public double Multi_TPO { get; set; }
        public double Multi_ROLL { get; set; }
        public double Multi_CUTT { get; set; }
        public double Multi_PACK { get; set; }
        public double Multi_STAMP { get; set; }
        public double Multi_FWRP { get; set; }
        public double Multi_SWRP { get; set; }
        public double Multi_GEN { get; set; }
        public double Multi_WRP { get; set; }
        public double Out { get; set; }
        //public double Attend { get; set; }
        public double In { get; set; }
        public double ValueHour { get; set; }
        public double ValuePeople { get; set; }
        public double ValuePeopleHour { get; set; }
        public Nullable<double> ActualWorker { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public double Register { get; set; }
        public Nullable<double> Absence_S { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
        public int CountGroupData { get; set; } //jumlah data
    }
}
