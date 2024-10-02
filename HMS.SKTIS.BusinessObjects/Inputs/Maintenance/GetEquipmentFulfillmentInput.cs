using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetEquipmentFulfillmentInput : BaseInput
    {
        public string RequestLocation { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestNumber { get; set; }
        public string Requestor { get; set; }
        public string ItemCode { get; set; }
        public DateTime? FulfillmentDate { get; set; }
    }
}
