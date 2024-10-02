using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetPlantEquipmentRepairsInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string ItemCode { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
