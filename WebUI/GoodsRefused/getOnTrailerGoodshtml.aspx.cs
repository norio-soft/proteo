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
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Orchestrator.WebUI.GoodsRefused
{
    public partial class getOnTrailerGoodshtml : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);

            // Get the data representing the goods stored on the trailer.
            Facade.ITrailer facTrailer = new Facade.Resource();
            DataSet dsGoods = facTrailer.GetGoodsStoredOnTrailer(resourceId);

            // Get the xml representation of the dataset and transform it ready for return to the client.
            string xml = dsGoods.GetXml();
            XmlDocument data = new XmlDocument();
            data.LoadXml(xml);
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load(Server.MapPath(@"..\xsl\goodsStoredOnTrailer.xsl"));
            
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
            
            XmlUrlResolver resolver = new XmlUrlResolver();
            XPathNavigator navigator = data.CreateNavigator();

            // Populate the Point.
            StringWriter sw = new StringWriter();
            transformer.Transform(navigator, args, sw);

            // Return the transformed xml to the client.
            Response.Write(sw.GetStringBuilder().ToString());
            Response.Flush();
            Response.End();
        }
    }
}
