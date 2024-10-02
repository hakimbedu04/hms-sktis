using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetEquipmentTransfersInput : BaseInput
    {
        public string SourceLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public DateTime? TransferDate { get; set; }
        public string UnitCodeDestination { get; set; }
    }
}
