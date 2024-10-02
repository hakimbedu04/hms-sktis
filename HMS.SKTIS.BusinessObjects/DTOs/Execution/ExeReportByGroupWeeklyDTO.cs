using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportByGroupWeeklyDTO
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

        public Nullable<int> Production { get; set; }

        public Nullable<float> TPKValue { get; set; }

        public Nullable<double> WorkHour { get; set; }

        public int WeekDay { get; set; }

        public double Register { get; set; }

        public Nullable<double> Absennce_A { get; set; }

        public Nullable<double> Absence_I { get; set; }

        public Nullable<double> Absence_C { get; set; }

        public Nullable<double> Absence_CH { get; set; }

        public Nullable<double> Absence_CT { get; set; }

        public Nullable<double> Absence_SLS { get; set; }

        public Nullable<double> Absence_SLP { get; set; }

        public Nullable<double> Absence_ETC { get; set; }

        public Nullable<double> Multi_TPO { get; set; }

        public Nullable<double> Multi_ROLL { get; set; }

        public Nullable<double> Multi_CUTT { get; set; }

        public Nullable<double> Multi_PACK { get; set; }

        public Nullable<double> Multi_STAMP { get; set; }

        public Nullable<double> Multi_FWRP { get; set; }

        public Nullable<double> Multi_SWRP { get; set; }

        public Nullable<double> Multi_GEN { get; set; }

        public Nullable<double> Multi_WRP { get; set; }

        public Nullable<double> Out { get; set; }

        public Nullable<double> Attend { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public Nullable<double> EmpIn { get; set; }

        public Nullable<float> ValueHour { get; set; }

        public Nullable<float> ValuePeople { get; set; }

        public Nullable<float> ValuePeopleHour { get; set; }

        public Nullable<double> Absence_S { get; set; }

        public Nullable<double> ActualWorker { get; set; }
    }
}
