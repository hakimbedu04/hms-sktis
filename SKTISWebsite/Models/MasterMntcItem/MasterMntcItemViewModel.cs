namespace SKTISWebsite.Models.MasterMntcItem
{
    public class MasterMntcItemViewModel : ViewModelBase
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemType { get; set; }
        public string UOM { get; set; }
        public string PriceType { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}