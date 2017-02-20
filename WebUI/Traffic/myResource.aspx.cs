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

using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;

namespace Orchestrator.WebUI.Traffic
{
    public partial class myResource : Orchestrator.Base.BasePage
    {
        private DataSet m_data = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDrivers();
            }
        }


        #region Drivers
        private void LoadDrivers()
        {
            int controlAreaId = 2;
            string trafficAreas = "24";
            DateTime startDate = new DateTime(2005, 12, 02, 00, 00, 00);
            DateTime endDate = new DateTime(2006, 02, 03, 23, 59, 59);
            bool onlyShowAvailable = false;

            using (Facade.IResource facResource = new Facade.Resource())
                m_data = facResource.GetDepotCodeResources(controlAreaId, trafficAreas, startDate, endDate, onlyShowAvailable,
                    Orchestrator.Globals.Configuration.DriverRequestHoursAhead);

            ((ComponentArt.Web.UI.Grid)NavBar.Items[0].Items[0].FindControl("dgDrivers")).DataSource = m_data.Tables[0];
            ((ComponentArt.Web.UI.Grid)NavBar.Items[0].Items[0].FindControl("dgDrivers")).DataBind();
            //ComponentArt.Web.UI.NavBarItem ni = NavBar1.FindItemById("i_Drivers");
            //((ComponentArt.Web.UI.Grid)ni.Items[0].FindControl("dgDrivers")).DataSource= m_data.Tables[0];
            //((ComponentArt.Web.UI.Grid)ni.Items[0].FindControl("dgDrivers")).DataBind();

        }


        protected string GetNext(int driverResourceId, int hasFuture)
        {
            string next= string.Empty;
            string tommorrow = string.Empty;

            if (hasFuture > 0)
                GetFuture(driverResourceId, hasFuture, out next, out tommorrow);

            if (next == "") return string.Empty;

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(next);
            StringBuilder sb = new StringBuilder();

            string link = "<a href=\"{0}\">{1}</a>";
            foreach (XmlElement el in xdoc.SelectNodes("//l[@group='next']"))
            {
                sb.Append(string.Format(link, el.Attributes["instructionId"].Value, el.Attributes["desc"].Value));
                sb.Append("<br/>");
            }

            return sb.ToString();
        }

        protected string GetTomorrow(int driverResourceId, int hasFuture)
        {
            string next = string.Empty;
            string tommorrow = string.Empty;

            if (hasFuture > 0)
                GetFuture(driverResourceId, hasFuture, out next, out tommorrow);

            if (next == "") return string.Empty;

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(tommorrow);
            StringBuilder sb = new StringBuilder();

            string link = "<a href=\"{0}\">{1}</a>";
            foreach (XmlElement el in xdoc.SelectNodes("//l[@group='tomorrow']"))
            {
                sb.Append(string.Format(link, el.Attributes["instructionId"].Value, el.Attributes["desc"].Value));
                sb.Append("<br/>");
            }

            return sb.ToString();
        }

        private void GetFuture(int resourceId, int hasFuture, out string next, out string tomorrow)
        {
            if (hasFuture == 0)
            {
                next = "&nbsp;";
                tomorrow = "&nbsp;";
            }
            else
            {
                // Recover this resource's future.
                Facade.IScheduleResource facScheduleResource = new Facade.Schedule();
                string futureXML = facScheduleResource.GetFutureXML(resourceId);

                // Load the XSLT.
                string xsltLocation = "~/xsl/ResourceFuture.xsl";
                //XmlTextReader xtr = new XmlTextReader(xsltLocation);
               // XslCompiledTransform xct = new XslCompiledTransform();
                //xct.Load(Server.MapPath(xsltLocation));

                // Load the XML to transform.
                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(futureXML);
                //XPathNavigator nav = doc.CreateNavigator();

                // Perform the transform for next.
                //XsltArgumentList args = new XsltArgumentList();
                //args.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
                //args.AddParam("mode", "", "next");

                //StringWriter sw = new StringWriter();
                //xct.Transform(nav, args, sw);
                next = futureXML;

                // Perform the transform for tomorrow.
                //args = new XsltArgumentList();
                //args.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
                //args.AddParam("mode", "", "tomorrow");

                //sw = new StringWriter();
                //xct.Transform(nav, args, sw);
                tomorrow = futureXML;
            }
        }
        #endregion
    }
    
}