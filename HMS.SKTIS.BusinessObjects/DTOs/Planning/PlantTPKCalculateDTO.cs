using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlantTPKCalculateDTO
    {
        public List<PlantTPKByProcessDTO> PlantTPKByProcess { get; set; }
        public List<TargetManualTPUDTO> PlantTPKTotals { get; set; }
    }
}
