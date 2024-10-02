using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningPlantTPK
{
    public class TPKStatusModel
    {
        public TPKStatusModel()
        {
            SubmitLog = false;
            Dates = null;
            Resubmit = false;
        }
        public bool SubmitLog { get; set; }
        public List<DateStateModel> Dates { get; set; }
        public bool Resubmit { get; set; }
    }
}