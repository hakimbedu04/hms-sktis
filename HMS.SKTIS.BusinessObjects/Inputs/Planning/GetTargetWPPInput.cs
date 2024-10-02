namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class GetTargetWPPInput
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string BrandCode { get; set; }
        public string LocationCode { get; set; }
        public int Shift { get; set; }
    }
}
