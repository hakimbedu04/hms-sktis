using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.News
{
    public class NewsInfoLevelViewModel
    {
        public List<Level0> DetailLevel0 { get; set; }

    }

    public class Level0
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public List<Level1> Level1Detail { get; set; }
    }

    public class Level1
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public List<Level2> Level2Detail { get; set; }
    }

    public class Level2
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public List<Level3> Level3Detail { get; set; }
    }

    public class Level3
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
    }


}