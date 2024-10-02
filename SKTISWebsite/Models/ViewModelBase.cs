using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models
{
    public class ViewModelBase
    {
        public string ResponseType { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
    }
}