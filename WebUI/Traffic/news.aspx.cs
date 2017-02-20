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
using System.Web.Caching;

public partial class Traffic_news : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        bool showNews = false;
        bool.TryParse(ConfigurationManager.AppSettings["RSS.TrafficNews"], out showNews);

        if (!showNews)
        {
            Rotator1.Visible = false;
            return;
        }

        try
        {
            DataSet dsNews = GetData();
            Rotator1.DataSource = dsNews;
            Rotator1.DataBind();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.Write(ex.Message);
        }
        
    }

    private DataSet GetData()
    {
        string cacheKey = "RSS.Traffic.Data";

        DataSet dsNews = null;
        if (Cache[cacheKey] == null)
        {
            XmlTextReader reader = new XmlTextReader(ConfigurationManager.AppSettings["RSS.TrafficNews.URL"]);
            dsNews = new DataSet();
            dsNews.ReadXml(reader);

            Cache.Add(cacheKey, dsNews, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 5, 0), CacheItemPriority.Normal, null);
        }
        else
        {
            dsNews = (DataSet)Cache[cacheKey];
        }

        return dsNews;
    }
}
