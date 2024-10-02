using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKTISWebsite.Models.Common;

namespace SKTISWebsite.Models.Common
{
    public class TPKPageResult<T1,T2> : ViewModelBase where T1 : class where T2 : class 
    {
        public TPKPageResult()
        {
            Results = new List<T1>();
            CustomResults = new List<T2>();
        }

        public List<T1> Results { get; set; }
        public List<T2> CustomResults { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}