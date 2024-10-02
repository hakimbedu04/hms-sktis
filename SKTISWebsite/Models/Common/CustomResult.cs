using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.Common
{
    public class CustomResult<T> : ViewModelBase where T : class
    {
        public List<T> CustomList { get; set; }
    }
}