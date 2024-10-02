using System;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetEquipmentRequestByIdInput
    {
        public DateTime RequestDate { get; set; }
        public string ItemCode { get; set; }
        public string LocationCode { get; set; }
        public string RequestNumber { get; set; }
    }
}
