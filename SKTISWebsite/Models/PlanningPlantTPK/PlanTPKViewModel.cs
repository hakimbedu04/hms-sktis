using System.Collections.Generic;

namespace SKTISWebsite.Models.PlanningPlantTPK
{
    public class PlanTPKViewModel : ViewModelBase
    {
        public PlanTPKViewModel()
        {
            PlantTPK = new List<PlantTPKModel>();
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
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string Shift { get; set; }

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
        public int? JKProcess1 { get; set; }
        public int? JKProcess2 { get; set; }
        public int? JKProcess3 { get; set; }
        public int? JKProcess4 { get; set; }
        public int? JKProcess5 { get; set; }
        public int? JKProcess6 { get; set; }
        public int? JKProcess7 { get; set; }

        public List<PlantTPKModel> PlantTPK { get; set; }

        //additional
        public bool ShowInWIPStock { get; set; }
    }
}
