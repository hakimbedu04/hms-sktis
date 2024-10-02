using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class WagesReportSummaryProductionCardDTO
    {
        public string Location { get; set; }
        public string Process { get; set; }
        public decimal? Produksi { get; set; }
        public decimal? UpahLain { get; set; }
        public string BrandGroupCode { get; set; }
    }
}
