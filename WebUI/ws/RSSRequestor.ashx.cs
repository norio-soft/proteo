using System.IO;
using System.Net;
using System.Web;
using System.Web.Services;

namespace Orchestrator.WebUI
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class RSSRequestor : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            //System.Threading.Thread.Sleep(10000); // simulate delay

            using (var wc = new WebClient())
            {
                using (var sr = new StreamReader(wc.OpenRead("http://www.highways.gov.uk/rssfeed/rss.xml")))
                {
                    context.Response.Write(sr.ReadToEnd());
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
