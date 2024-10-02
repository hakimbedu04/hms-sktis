using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.PlantWages
{
    public class MailPlantWagesInput
    {
        public string FromName { get; set; }
        public string FromEmailAddress { get; set; }
        public string ToName { get; set; }
        public string ToEmailAddress { get; set; }
        public string Subject { get; set; }
        public string BodyEmail { get; set; }
    }
}
