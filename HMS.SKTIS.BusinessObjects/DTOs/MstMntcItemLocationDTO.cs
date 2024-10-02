using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstMntcItemLocationDTO
    {
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string ItemType { get; set; }
        public int BufferStock { get; set; }
        public int MinOrder { get; set; }
        public int StockReadyToUse { get; set; }
        public int StockAll { get; set; }
        public int AVGWeeklyUsage { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
