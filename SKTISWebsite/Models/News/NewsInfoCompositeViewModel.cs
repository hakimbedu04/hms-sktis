using System;
using System.ComponentModel.DataAnnotations;

namespace SKTISWebsite.Models.News
{
    public class NewsInfoCompositeViewModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
        public bool IsSlider { get; set; }
        public bool IsInformatips { get; set; }
        public bool Active { get; set; }
        public string Location { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
    }
}