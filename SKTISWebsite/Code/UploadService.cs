using SKTISWebsite.Models.MstTPOPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Code
{
    public class UploadService : IUploadService
    {
        public async Task<JsonResult> Upload(string modulePath = "")
        {
            string uploadPath = "~/upload";
            string fileName = "";
            string physicalPath = "";
            string absolutePath = "";
            string AbsoluteName = null;
            try
            {
                foreach (string file in HttpContext.Current.Request.Files)
                {
                    var fileContent = HttpContext.Current.Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        string extension = Path.GetExtension(file);
                        fileName = DateTime.Now.Ticks + extension;
                        physicalPath = HttpContext.Current.Server.MapPath(uploadPath);
                        if (modulePath != "")
                        {
                            physicalPath = Path.Combine(physicalPath, modulePath);
                            if (!Directory.Exists(physicalPath))
                            {
                                Directory.CreateDirectory(physicalPath);
                            }
                            uploadPath += "/" + modulePath;
                        }
                        physicalPath = Path.Combine(physicalPath, fileName);
                        absolutePath = uploadPath + "/" + fileName;
                        AbsoluteName = fileName;
                        using (var fileStream = System.IO.File.Create(physicalPath))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult() { Data = new UploadViewModel() { ResponseType = "KO", Message = ex.Message, Filename = absolutePath } };
                //return Json(new UploadViewModel() { ResponseType = "KO", Message = ex.Message, Filename = path });
            }
            return new JsonResult() { Data = new UploadViewModel() { ResponseType = "OK", Message = "File uploaded successfully", Filename = absolutePath, AbsoluteName = AbsoluteName } };
            //return Json(new UploadViewModel() { ResponseType = "OK", Message = "File uploaded successfully", Filename = path });
        }
    }
}