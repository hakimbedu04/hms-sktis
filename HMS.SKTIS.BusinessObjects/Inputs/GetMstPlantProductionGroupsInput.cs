namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstPlantProductionGroupsInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessSettingsCode { get; set; }
    }
}
