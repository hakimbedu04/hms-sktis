using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesReportSummaryProductionCard
{
    public class WagesReportSummaryViewModel : ViewModelBase
    {
        public WagesReportSummaryViewModel()
        {
            ProductionCards = new List<WagesReportSummaryProductionCardViewModel>();
            
        }
        public string Location { get; set; }
        public List<WagesReportSummaryProductionCardViewModel> ProductionCards { get; set; }
        public string TotalProduksiString { get; set; }
        public string TotalUpahLainString { get; set; }

        public string TotalProduksiCorrectionString { get; set; }
        public string TotalUpahLainCorrectionString { get; set; }

        public decimal TotalProduksi { get; set; }
        public decimal TotalUpahLain { get; set; }
               
        public decimal TotalProduksiCorrection { get; set; }
        public decimal TotalUpahLainCorrection { get; set; }

        public bool IsProductionCard { get; set; }
    }
}