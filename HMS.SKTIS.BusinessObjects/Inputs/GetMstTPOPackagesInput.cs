using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetMstTPOPackagesInput : BaseInput
    {
        public string LocationCode { get; set; }
        public string Year { get; set; }
        public string BrandGroupCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
