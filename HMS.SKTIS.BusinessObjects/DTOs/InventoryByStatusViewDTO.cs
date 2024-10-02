namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class InventoryByStatusViewDTO
    {
        public InventoryByStatusViewDTO()
        {
            ReadyToUse = 0;
            OnUse = 0;
            OnRepair = 0;
        }
        public int? ReadyToUse { get; set; }
        public int? OnUse { get; set; }
        public int? OnRepair { get; set; }
    }
}
