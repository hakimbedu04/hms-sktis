using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ExeReportByGroup
{
    public class ExeReportByGroupWeeklyViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string StatusEmp { get; set; }
        public int KPSWeek { get; set; }
        public int KPSYear { get; set; }
        public int Shift { get; set; }
        public int Production { get; set; }
        public float? TPKValue { get; set; }
        public int WorkHour { get; set; }
        public int WeekDay { get; set; }
        public int Register { get; set; }
        public double Absennce_A { get; set; }
        public double Absence_I { get; set; }
        public double Absence_C { get; set; }
        public double Absence_CH { get; set; }
        public double Absence_CT { get; set; }
        public double Absence_SLS { get; set; }
        public double Absence_SLP { get; set; }
        public double Absence_ETC { get; set; }
        public int Multi_TPO { get; set; }
        public int Multi_ROLL { get; set; }
        public int Multi_CUTT { get; set; }
        public int Multi_PACK { get; set; }
        public int Multi_STAMP { get; set; }
        public int Multi_FWRP { get; set; }
        public int Multi_SWRP { get; set; }
        public int Multi_GEN { get; set; }
        public int Multi_WRP { get; set; }
        public int Out { get; set; }
        public int Attend { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
