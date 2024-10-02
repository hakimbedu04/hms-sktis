using System;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetEquipmentRequestsInput : BaseInput
    {
        public string LocationCode { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestNumber { get; set; }
        public string Requestor { get; set; }
        public string ItemCode { get; set; }
    }
}
