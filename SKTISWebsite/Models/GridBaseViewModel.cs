using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SKTISWebsite.Models
{
    public class GridBaseViewModel
    {
        public GridBaseViewModel()
        {
            PageSize = 10;
            PageSizeSelectList = new SelectList(new List<int> { 1, 10, 20, 30, 40, 50 });
        }
        public int PageSize { get; set; }
        public SelectList PageSizeSelectList { get; set; }
        public string SortOrder { get; set; }
        public string SortExpression { get; set; }
    }
}
