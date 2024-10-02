using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface IVTLogger
    {
        void Err(Exception err, List<object> obj = null, string message = null);
    }
}
