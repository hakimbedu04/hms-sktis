using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.Common
{
    public class CustomInsertUpdateData<T1, T2>
        where T1 : class
        where T2 : class
    {
        public IDictionary<string, string> Parameters { get; set; }
        public List<T1> New { get; set; }
        public List<T1> Edit { get; set; }
        public List<T2> Total { get; set; }

        public string[] ExpelledGroup { get; set; }
    }
}