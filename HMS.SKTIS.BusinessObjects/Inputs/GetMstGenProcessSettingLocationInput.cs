namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstGenProcessSettingLocationInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public int? IDProcess { get; set; }
        public string ProcessGroup { get; set; }
    }
}
