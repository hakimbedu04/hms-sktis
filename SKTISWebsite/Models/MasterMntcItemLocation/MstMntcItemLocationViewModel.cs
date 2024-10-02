using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterMntcItemLocation
{
    public class MstMntcItemLocationViewModel : ViewModelBase
    {
        public string ItemLocationCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemType { get; set; }
        public string LocationCode { get; set; }
        public int BufferStock { get; set; }
        public int MinOrder { get; set; }
        public int StockReadyToUse { get; set; }
        public int StockAll { get; set; }
        public int AVGWeeklyUsage { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}