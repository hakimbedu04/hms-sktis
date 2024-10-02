using HMS.SKTIS.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.BLL
{
    public partial class VTLogger : IVTLogger
    {
        public void Err(Exception err, List<object> obj = null, string message = null)
        {
            string path = ConstructErrorPath();

            using (StreamWriter writetext = File.AppendText(path))
            {
                var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

                #region General
                writetext.WriteLine("####################################################################################################################################");
                writetext.WriteLine("# Version          : " + GetVersion());
                writetext.WriteLine("# Log time         : " + DateTime.Now);
                writetext.WriteLine("# Url              : " + HttpContext.Current.Request.Url.AbsoluteUri);
                writetext.WriteLine("# Exception        : " + err.Message);
                writetext.WriteLine("# InnerException   : " + JsonConvert.SerializeObject(err.InnerException));

                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

                if ( routeValues != null & routeValues.ContainsKey("action") )
                {
                    writetext.WriteLine("# Action       : " + routeValues["action"]);
                }

                if ( message != null )
                {
                    writetext.WriteLine("# Message      : " + message);
                }

                writetext.WriteLine("####################################################################################################################################");

                #endregion

                #region Stack Trace
                writetext.WriteLine(string.Empty);
                writetext.WriteLine("# Stack Trace:");
                writetext.WriteLine(err.ToString());
                writetext.WriteLine(string.Empty);
                #endregion

                #region Session
                if (session != null)
                {
                    writetext.WriteLine("# Session:");
                    writetext.WriteLine(string.Format("Username         : {0}", session.Username));
                    writetext.WriteLine(string.Format("Locations        : {0}", string.Join(",", session.Location.Select(s => s.Code).ToArray())));
                    writetext.WriteLine(string.Format("Responsibility   : {0}", session.Responsibility.ResponsibilityName));
                    writetext.WriteLine(string.Empty);
                }
                #endregion

                #region Objects
                if (obj != null && obj.Any())
                {
                    writetext.WriteLine("# Serialized Objects:");

                    for (var i = 0; i < obj.Count; i++)
                    {
                        writetext.WriteLine(string.Empty);
                        writetext.WriteLine("> " + obj[i].GetType().ToString());
                        writetext.WriteLine(string.Empty);
                        writetext.WriteLine(JsonConvert.SerializeObject(obj[i]));
                    }
                }
                #endregion

                writetext.WriteLine("==================================================================================================================================");

                writetext.WriteLine(string.Empty);
                writetext.WriteLine("+");
                writetext.WriteLine(string.Empty);
            }
        }

        private string ConstructErrorPath()
        {
            var subPath = "~/Logs/Errors/" + DateTime.Now.ToString("yyyyMM");

            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            var page = "ALL";

            if (routeValues != null)
            {
                if (routeValues.ContainsKey("controller"))
                {
                    page = routeValues["controller"].ToString();
                }
            }

            var fileName = String.Format("{0}_{1}.{2}", page, DateTime.Now.ToString("yyyyMMdd"), "log");

            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));

            try
            {
                if (!exists)
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
            }
            catch (Exception e)
            {
                // (╯°□°)╯︵ ┻━┻
                //throw e;
            }
            

            return HttpContext.Current.Server.MapPath(Path.Combine(subPath, fileName));
        }

        private string GetVersion()
        {
            var filename = "~/version";

            bool exists = System.IO.File.Exists(HttpContext.Current.Server.MapPath(filename));

            if (exists)
                return System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(filename));
            else
                return string.Empty;
        }
    }
}
