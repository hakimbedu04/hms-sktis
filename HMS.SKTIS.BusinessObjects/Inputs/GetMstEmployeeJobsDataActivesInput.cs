namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstEmployeeJobsDataActivesInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string ProcessSettingCode { get; set; }
        public string GroupCode { get; set; }
        public string EmployeeID { get; set; }
    }
}
