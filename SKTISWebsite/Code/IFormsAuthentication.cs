using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Code
{
    public interface IFormsAuthentication
    {
        void SignIn(string username, bool createPersistentCookie);
        void SignOut();
    }
}
