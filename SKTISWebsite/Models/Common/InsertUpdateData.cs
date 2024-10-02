using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.Common
{
    public class InsertUpdateData<T> where T : class
    {
        public IDictionary<string, string> Parameters { get; set; }
        public List<T> New { get; set; }
        public List<T> Edit { get; set; }
    }
}