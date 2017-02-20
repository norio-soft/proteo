using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Orchestrator.WebUI.GPS.DataProviders
{
    public partial class getTrafficProblemsGeoRSS : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigurationManager.AppSettings["RSS.TrafficNews.URL"]);

                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(Server.MapPath(@"XSL\HighwaysAgencyTrafficProblemsToGeoRSS.xsl"));

                XmlUrlResolver resolver = new XmlUrlResolver();
                XPathNavigator navigator = doc.CreateNavigator();

                using (TextWriter textWriter = new StreamWriter(Response.OutputStream, Encoding.UTF8))
                {
                    XmlTextWriter xmlWriter = new XmlTextWriter(textWriter);
                    transformer.Transform(navigator, null, xmlWriter);
                }
            }
            catch { }
        }
    }
}