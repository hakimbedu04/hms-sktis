using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public List<LocationUser> Location { get; set; }
        public Responsibility Responsibility { get; set; }
    }
    public class LocationUser
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
