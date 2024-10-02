using System.Collections.Generic;

namespace SKTISWebsite.Models.PlanningTPOTPK
{
    public class PlanTPOTPKViewModel : ViewModelBase
    {
        public PlanTPOTPKViewModel()
        {
            PlanTPOTPK = new List<PlanTPOTPKModel>();
            WIPPreviousValue = 0;
            WIPStock1 = 0;
            WIPStock2 = 0;
            WIPStock3 = 0;
            WIPStock4 = 0;
            WIPStock5 = 0;
            WIPStock6 = 0;
            WIPStock7 = 0;
            JKProcess1 = 0;
            JKProcess2 = 0;
            JKProcess3 = 0;
            JKProcess4 = 0;
            JKProcess5 = 0;
            JKProcess6 = 0;
            JKProcess7 = 0;
        }

        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
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

        //jk process
        public float? JKProcess1 { get; set; }
        public float? JKProcess2 { get; set; }
        public float? JKProcess3 { get; set; }
        public float? JKProcess4 { get; set; }
        public float? JKProcess5 { get; set; }
        public float? JKProcess6 { get; set; }
        public float? JKProcess7 { get; set; }

        public List<PlanTPOTPKModel> PlanTPOTPK { get; set; }

        //additional
        public bool ShowInWIPStock { get; set; }
        public string UnitCode { get; set; }

        public float TargetWPP { get; set; }
    }
}