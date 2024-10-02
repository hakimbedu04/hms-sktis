using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
   public class WagesReportSummaryDTO
    {
       public WagesReportSummaryDTO()
        {
            ProductionCards = new List<WagesReportSummaryProductionCardDTO>();
            
        }
        public string Location { get; set; }
        public List<WagesReportSummaryProductionCardDTO> ProductionCards { get; set; }

        public decimal? TotalProduksi { get; set; }
        public decimal? TotalUpahLain { get; set; }

        public decimal? TotalProduksiCorrection { get; set; }
        public decimal? TotalUpahLainCorrection { get; set; }

        public bool IsProductionCard { get; set; }
    }
}
