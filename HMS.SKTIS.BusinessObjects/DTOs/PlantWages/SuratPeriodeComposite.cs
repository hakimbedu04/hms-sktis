using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class SuratPeriodeComposite
    {
        public bool Status { get; set; }
        public string AlphaDate { get; set; }
        public string AbsentType { get; set; }
        public string Remark { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}
