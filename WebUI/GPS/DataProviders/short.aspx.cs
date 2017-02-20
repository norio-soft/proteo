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

using System.Xml;

namespace Orchestrator.WebUI.GPS.DataProviders
{
    public partial class GetStartingPointsForPlannableLegs : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Server.Transfer("gettrafficproblemsgeorss.aspx");
            DataSet ds = null;
            decimal lat = -1;
            decimal lon = -1;
            decimal range = 0;
            int nextMinutes = -1;

            if (!int.TryParse(Request.QueryString["NextMins"], out nextMinutes))
                nextMinutes = -1;
            Facade.IGPS facGPS = new Facade.GPS();

            if (
                decimal.TryParse(Request.QueryString["Lat"], out lat) && 
                decimal.TryParse(Request.QueryString["Long"], out lon) && 
                decimal.TryParse(Request.QueryString["Range"], out range))
            {
                // Within a range of a point.
                ds = facGPS.GetPlannableLegs(nextMinutes, lat, lon, range);
            }
            else
            {
                // All jobs.
                ds = facGPS.GetPlannableLegs(nextMinutes);
            }

            if (ds != null)
            {
                // Convert this to RSS
                XmlDocument doc = new XmlDocument();
                XmlElement rssEl = doc.CreateElement("rss");

                rssEl.SetAttribute("version", "2.0");
                rssEl.SetAttribute("xmlns:geo", "http://www.w3.org/2003/01/geo/wgs84_pos#");
                doc.AppendChild(rssEl);
                XmlElement channelEl = doc.CreateElement("channel");
                XmlElement channelTitleEl = doc.CreateElement("title");
                channelTitleEl.InnerText = "Plannable Leg Start Positions";
                XmlElement channelLinkEl = doc.CreateElement("link");
                channelLinkEl.InnerText = "#";
                XmlElement channelDescEl = doc.CreateElement("description");
                channelDescEl.InnerText = "Created By P1 Technology Partners";

                channelEl.AppendChild(channelTitleEl);
                channelEl.AppendChild(channelLinkEl);
                channelEl.AppendChild(channelDescEl);

                rssEl.AppendChild(channelEl);

                XmlElement itemEl = null;
                XmlElement itemTitle = null;
                XmlElement itemLink = null;
                XmlElement itemDescription = null;
                XmlElement geoLat = null;
                XmlElement geoLong = null;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    try
                    {
                        itemEl = doc.CreateElement("item");
                        geoLat = doc.CreateElement("geo:lat", "http://www.w3.org/2003/01/geo/wgs84_pos#");
                        geoLat.InnerText = row["WGS84Latitude"].ToString();
                        geoLong = doc.CreateElement("geo:long", "http://www.w3.org/2003/01/geo/wgs84_pos#");
                        geoLong.InnerText = row["WGS84Longitude"].ToString();

                        itemTitle = doc.CreateElement("title");
                        itemTitle.InnerText = row["Description"].ToString().Trim();// +" " + ((DateTime)row["LegPlannedStartDateTime"]).ToString("dd/MM/yy HH:mm");

                        itemDescription = doc.CreateElement("description");
                        itemDescription.InnerText = "Job: " + row["JobID"].ToString();

                        itemEl.AppendChild(itemTitle);

                        itemLink = doc.CreateElement("link");
                        itemLink.InnerText = Globals.Configuration.WebServer + "/job/job.aspx?jobid=" + row["JobID"].ToString();

                        itemEl.AppendChild(itemDescription);
                        itemEl.AppendChild(itemLink);
                        itemEl.AppendChild(geoLat);
                        itemEl.AppendChild(geoLong);

                        channelEl.AppendChild(itemEl);
                    }
                    catch { }
                }

                // Stream the results to the client
                Response.Clear();
                Response.ContentType = "text/xml";
                Response.Write(doc.OuterXml.ToString());
            }
        }
    }
}