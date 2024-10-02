namespace SKTISWebsite.Models.MasterGenList
{
    public class MasterGenListViewModel : ViewModelBase
    {
        public string ListGroup { get; set; }
        public string ListDetail { get; set; }
        public bool StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}