using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class CalculatePlantTPKInput
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public string UnitCode { get; set; }
        public List<PlantTPKByProcessDTO> ListPlantTPK { get; set; }
        public List<TargetManualTPUDTO> TotalPlantTPK { get; set; }
        public DateTime FilterCurrentDayForward { get; set; }
        public bool IsFilterCurrentDayForward { get; set; }
    }
}
