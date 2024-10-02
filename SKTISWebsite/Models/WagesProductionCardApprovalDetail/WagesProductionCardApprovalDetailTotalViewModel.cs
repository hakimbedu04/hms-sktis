using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesProductionCardApprovalDetail
{
    public class WagesProductionCardApprovalDetailTotalViewModel
    {
        public List<WagesProductionCardApprovalDetailGridViewModel> ProdcardApprovalDetailGridView { get; set; }
        public int? TotalGroupBrand { get; set; }
        public int? TotalWorkerBrand { get; set; }
        public float? TotalProduksiBrand { get; set; }
        public float? TotalUpahLainBrand { get; set; }
    }
}