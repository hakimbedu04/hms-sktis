using System.Collections.Generic;
using System.Linq;
using HMS.SKTIS.BusinessObjects.Inputs;

namespace SKTISWebsite.Models.Common
{
    public class PageResult<T> where T : class
    {
        public List<T> Results { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PageResult()
        {

        }

        public PageResult(List<T> list, BaseInput criteria)
        {
            TotalRecords = list.Count;
            TotalPages = (list.Count / criteria.PageSize) + (list.Count % criteria.PageSize != 0 ? 1 : 0);
            Results = list.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
        }

        public PageResult(List<T> list)
        {
            TotalRecords = list.Count;
            TotalPages = 1;
            Results = list.ToList();
        }
    }
}