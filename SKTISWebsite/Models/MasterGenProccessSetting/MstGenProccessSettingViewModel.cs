namespace SKTISWebsite.Models.MasterGenProccessSetting
{
    public class MstGenProccessSettingViewModel : ViewModelBase
    {
        public int IDProcess { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }        
        public int? StdStickPerHour { get; set; }
        public int? MinStickPerHour { get; set; }
        public int? UOMEblek { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int ProcessOrder { get; set; }
        public int? MaxWorker { get; set; }
    }
}