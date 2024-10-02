namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstGenLocationInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string ParentLocationCode { get; set; }
    }
}
