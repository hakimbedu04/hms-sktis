using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SKTISWebsite.Models.UtilTransactionLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.PlanningPlantTPU
{
    public class TPUStatusModel
    {
        public TPUStatusModel()
        {
            SubmitLog = null;
            Dates = null;
            Resubmit = false;
        }
        public UtilTransactionLogViewModel SubmitLog { get; set; }
        public List<DateStateModel> Dates { get; set; }
        public bool Resubmit { get; set; }
    }
}