using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class SuratPeriodeLaluInput : BaseInput
    {
        public bool Status { get; set; }
        public DateTime AlphaDate { get; set; }
        public string AbsentType { get; set; }
    }
}
