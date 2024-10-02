using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetEquipmentReceivesInput : BaseInput
    {
        public string SourceLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public DateTime? TransferDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
    }
}
