using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeReportByGroupViewDTO
    {
        public string BrandGroupCode { get; set; }

        public string LocationCode { get; set; }

        public string UnitCode { get; set; }

        public string GroupCode { get; set; }

        public Nullable<double> Register { get; set; }

        public Nullable<double> A { get; set; }

        public Nullable<double> I { get; set; }

        public Nullable<double> S { get; set; }

        public Nullable<double> C { get; set; }

        public Nullable<double> CH { get; set; }

        public Nullable<double> CT { get; set; }

        public Nullable<double> SLSSLP { get; set; }

        public Nullable<double> ETC { get; set; }

        public Nullable<double> Multi_TPO { get; set; }

        public Nullable<double> Multi_ROLL { get; set; }

        public Nullable<double> Multi_CUTT { get; set; }

        public Nullable<double> Multi_PACK { get; set; }

        public Nullable<double> Multi_STAMP { get; set; }

        public Nullable<double> Multi_FWRP { get; set; }

        public Nullable<double> Multi_SWRP { get; set; }

        public Nullable<double> Multi_GEN { get; set; }

        public Nullable<double> Multi_WRP { get; set; }

        public Nullable<double> In { get; set; }

        public Nullable<double> Out { get; set; }

        public Nullable<double> ActualWorker { get; set; }

        public Nullable<double> WorkHour { get; set; }

        public Nullable<decimal> Production { get; set; }

        public Nullable<decimal> ValuePeople { get; set; }

        public Nullable<decimal> ValueHour { get; set; }

        public Nullable<decimal> ValuePeopleHour { get; set; }

        public System.DateTime ProductionDate { get; set; }
    }
}
