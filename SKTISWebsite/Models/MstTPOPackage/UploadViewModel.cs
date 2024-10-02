using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MstTPOPackage
{
    public class UploadViewModel : ViewModelBase
    {
        public UploadViewModel()
        {
            AbsoluteName = null;
        }
        public string Filename { get; set; }
        public string AbsoluteName { get; set; }
    }
}