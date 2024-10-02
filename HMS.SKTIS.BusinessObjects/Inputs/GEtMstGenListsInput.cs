namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstGenListsInput : BaseInput
    {
        public string ListGroup { get; set; }
        public string[] ListDetail { get; set; }
    }
}
