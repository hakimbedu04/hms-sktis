namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetPlanTPUsInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int? Shift { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
        public string Conversion { get; set; }
    }
}
