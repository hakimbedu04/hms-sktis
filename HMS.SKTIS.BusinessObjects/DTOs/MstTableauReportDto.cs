using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstTableauReportDto
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string ReportUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
