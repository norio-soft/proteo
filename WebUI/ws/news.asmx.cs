using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

using System.Xml;
using System.Data;
using System.Configuration;
using System.Web.Script.Services;

namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for news
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class news : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetNews()
        {
            string retVal = string.Empty;

            try
            {
                XmlTextReader reader = new XmlTextReader(ConfigurationManager.AppSettings["RSS.TrafficNews.URL"]);
                DataSet dsNews = new DataSet();
                dsNews.ReadXml(reader);

                // Build the divs for the return;
                string newsItem = @"<div onmousedown=""window.open('{0}')""><a>{1}</a><div id=""title"">{2}</div><div id=""newsItem"">{3}</div></div>";
                

                foreach (DataRow row in dsNews.Tables[2].Rows)
                {
                    retVal += string.Format(newsItem, row["link"].ToString(), DateTime.Parse(row["pubdate"].ToString()).ToString("dd/MM HH:mm"), row["title"].ToString(), row["description"].ToString());
                }
            }
            catch
            {

            }
            return retVal;
        }
    }
}
