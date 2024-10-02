namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class WIPStockDTO
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string ProcessGroup { get; set; }
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public int? WIPCurrentValue { get; set; }
        public int? WIPPreviousValue { get; set; }
        public int? WIPStock1 { get; set; }
        public int? WIPStock2 { get; set; }
        public int? WIPStock3 { get; set; }
        public int? WIPStock4 { get; set; }
        public int? WIPStock5 { get; set; }
        public int? WIPStock6 { get; set; }
        public int? WIPStock7 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
