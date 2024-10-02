using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SKTISWebsite.Code
{
    public interface IUploadService
    {
        Task<JsonResult> Upload(string modulePath = "");
    }
}
