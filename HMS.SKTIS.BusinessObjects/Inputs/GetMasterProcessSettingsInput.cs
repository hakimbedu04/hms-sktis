namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMasterProcessSettingsInput : BaseInput
    {
        public string BrandCode { get; set; }
        public string Process { get; set; }
        public int? IDProcess { get; set; }
    }
}
