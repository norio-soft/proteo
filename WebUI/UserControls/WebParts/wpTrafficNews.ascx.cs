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

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpTrafficNews : System.Web.UI.UserControl, IWebPart
    {
        #region WebPart Support
        #region Private fields
        private String _catalogImageUrl = string.Empty;
        private String _description = string.Empty;
        private String _subTitle = "[0]";
        private String _title = "Traffic News";

        private DateTime _startDate = DateTime.MinValue;
        private DateTime _endDate = DateTime.MaxValue;

        #endregion

        #region Public Properties
        public String CatalogIconImageUrl
        {
            get { return _catalogImageUrl; }
            set { _catalogImageUrl = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public String Subtitle
        {
            get { return string.Empty; }
            set { ; }
        }
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public String TitleIconImageUrl
        {
            get { return string.Empty; }
            set { ; }
        }
        public String TitleUrl
        {
            get { return string.Empty; }
            set { ; }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            set { _endDate = value; }
            get { return _endDate; }
        }

        #endregion
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("wpTrafficNews Loading");
            try
            {
                Rotator1.Visible = true;
                DataSet dsNews = GetData();

                if (dsNews.Tables[3].Rows.Count > 1)
                {
                    Rotator1.DataSource = dsNews.Tables[3];
                    Rotator1.DataBind();

                }
                else
                {
                    Rotator1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                Rotator1.Visible = false;
            }
            Trace.Write("wpTrafficNews Complete");
        }

        private DataSet GetData()
        {
            string cacheKey = "RSS.Traffic.Data.WebPart";

            DataSet dsNews = null;
            if (Cache[cacheKey] == null)
            {
                XmlTextReader reader = new XmlTextReader(ConfigurationManager.AppSettings["RSS.TrafficNews.URL"]);
                dsNews = new DataSet();
                dsNews.ReadXml(reader);

                Cache.Add(cacheKey, dsNews, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 15, 0), CacheItemPriority.Normal, null);
            }
            else
            {
                dsNews = (DataSet)Cache[cacheKey];
            }

            return dsNews;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }


    }
}