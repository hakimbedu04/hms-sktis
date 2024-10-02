using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class NewsInfoCompositeDTO
    {
        public int NewsId { get; set; }
        public string TitleID { get; set; }
        public string TitleEN { get; set; }
        public string ShortDescriptionID { get; set; }
        public string ShortDescriptionEN { get; set; }
        public string Image { get; set; }
        public bool IsSlider { get; set; }
        public bool IsInformatips { get; set; }
        public bool Active { get; set; }
        public string Location { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
    }
}
