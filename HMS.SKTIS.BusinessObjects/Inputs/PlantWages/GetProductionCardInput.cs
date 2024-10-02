using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class GetProductionCardInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string Unit { get; set; }
        public int Shift { get; set; }
        public string Process { get; set; }
        public string Group { get; set; }
        public string Brand { get; set; }
        public string BrandGroupCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public DateTime Date { get; set; }
        public int RevisionType { get; set; }
        public DateTime starDate { get; set; }
        public DateTime endDate { get; set; }
        public string UserName { get; set; }
    }
}
