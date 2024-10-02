using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class NewsInfoDTO
    {
        public int NewsId { get; set; }
        public string TitleID { get; set; }
        public string TitleEN { get; set; }
        public string ShortDescriptionID { get; set; }
        public string ShortDescriptionEN { get; set; }
        public string ContentID { get; set; }
        public string ContentEN { get; set; }
        public string Image { get; set; }
        public bool? IsSlider { get; set; }
        public bool? IsInformatips { get; set; }
        public bool? Active { get; set; }
        public string Location { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
