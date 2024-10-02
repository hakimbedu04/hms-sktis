using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesProductionCardApproval
{
    public class WagesProductionCardApprovalViewModel
    {
        public int? RevisionType { get; set; }
        public string ProductionCardCode { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public string ProductionDate { get; set; }
        public int? IDFlow { get; set; }
        public string Status { get; set; }
        public string UserAD { get; set; }
        public int IDRole { get; set; }
        public string RolesName { get; set; }
        public int Shift { get; set; }
    }
}