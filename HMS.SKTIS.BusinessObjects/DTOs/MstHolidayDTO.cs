using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstHolidayDTO
    {
        public DateTime HolidayDate { get; set; }
        public string HolidayType { get; set; }
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
