using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class ProductionCardCalculateTotalPayotherDTO
    {
        public string Group { get; set; }
        public string ActualProduction { get; set; }
        public string UpahLain { get; set; }

        public int RevisionType { get; set; }
        
    }
}
