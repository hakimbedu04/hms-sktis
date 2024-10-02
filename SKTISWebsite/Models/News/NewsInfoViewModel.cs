using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.News
{
    public class NewsInfoViewModel
    {
        public int NewsId { get; set; }
        public string TitleID { get; set; }
        public string TitleEN { get; set; }

        [DataType(DataType.MultilineText)]
        public string ShortDescriptionID { get; set; }
        public string ShortDescriptionEN { get; set; }

        [UIHint("TinyMCERichTextEditor"), AllowHtml]
        public string ContentID { get; set; }

        [UIHint("TinyMCERichTextEditor"), AllowHtml]
        public string ContentEN { get; set; }
        public string Image { get; set; }
        public bool IsSlider { get; set; }
        public bool IsInformatips { get; set; }
        public bool Active { get; set; }
        public string Location { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
    }

    public class NewsInfoFormModel{
        public NewsInfoViewModel Details { get; set; }
        public HttpPostedFileBase ImageUpload { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}