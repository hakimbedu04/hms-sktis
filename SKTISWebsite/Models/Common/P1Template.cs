using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.Common
{
    public class P1Template<T1>
        where T1 : class
    {
        public List<T1> Data { get; set; }
    }
}