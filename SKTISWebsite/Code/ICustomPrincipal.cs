using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Code
{
    public interface ICustomPrincipal : IPrincipal
    {
        string Username { get; set; }
    }
}
