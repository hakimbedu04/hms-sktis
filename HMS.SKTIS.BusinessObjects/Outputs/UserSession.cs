using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace HMS.SKTIS.BusinessObjects.Outputs
{
    public class UserSession
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public List<UserSessionLocation> Location { get; set; }
        public Responsibility Responsibility { get; set; }
    }

    public class UserSessionLocation
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
