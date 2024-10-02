namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetTargetManualTPUInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public int? Shift { get; set; }
    }
}
