using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.News
{
    public class NewsInfoDetailsViewModel
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        [UIHint("TinyMCERichTextEditor"), AllowHtml]
        public string Content { get; set; }
        public string Image { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime UpdatedDate { get; set; }
        public string UpdateBy { get; set; }
    }
}