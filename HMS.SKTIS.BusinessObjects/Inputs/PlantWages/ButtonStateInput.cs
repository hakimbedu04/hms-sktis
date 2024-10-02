using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class ButtonStateInput
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string Shift { get; set; }
        public string Date { get; set; }
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public int RoleId { get; set; }
        public string revisiontype { get; set; }
        public string currentDate { get; set; } 
    }
}
