using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class TPOTPKByProcessDTO
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string ProdGroup { get; set; }
        public string ProcessGroup { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }

        //wip stock
        public int? WIPPreviousValue { get; set; }
        public int? WIPStock1 { get; set; }
        public int? WIPStock2 { get; set; }
        public int? WIPStock3 { get; set; }
        public int? WIPStock4 { get; set; }
        public int? WIPStock5 { get; set; }
        public int? WIPStock6 { get; set; }
        public int? WIPStock7 { get; set; }        

        public List<TPOTPKDTO> PlanTPOTPK { get; set; }

        //additional
        public string UpdatedBy { get; set; }

        public string UnitCode { get; set; }
        public int? VarianceWIP1
        {
            get
            {
                return WIPStock1 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP2
        {
            get
            {
                return WIPStock2 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP3
        {
            get
            {
                return WIPStock3 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP4
        {
            get
            {
                return WIPStock4 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP5
        {
            get
            {
                return WIPStock5 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP6
        {
            get
            {
                return WIPStock6 - WIPPreviousValue;
            }
        }
        public int? VarianceWIP7
        {
            get
            {
                return WIPStock7 - WIPPreviousValue;
            }
        }
    }
}
