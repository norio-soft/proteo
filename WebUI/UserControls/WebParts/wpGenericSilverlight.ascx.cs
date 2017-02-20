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

using Orchestrator;
using System.Web.Caching;

using Telerik.Web.UI;

using Telerik.Charting;

using System.Drawing;
namespace Orchestrator.WebUI.WebParts
{
    public partial class wpGenericSilverlight: System.Web.UI.UserControl, IWebPart
    {
        #region Web Part Support
        #region Private fields
        
        private String _catalogImageUrl = string.Empty;
        private String _description = string.Empty;
        private String _subTitle = string.Empty;
        private String _title = string.Empty;

        private DateTime _startDate = DateTime.MinValue;
        private DateTime _endDate = DateTime.MaxValue;

        //public enum eDisplayOption { Last_30_Days, This_Week, This_Month, This_Year };
        //private eDisplayOption _displayOption = eDisplayOption.This_Week;

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

        public String Xap { get; set; }
        public String Xaml { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public String CacheProofXap 
        { 
            get
            {
                //Get the file creation date of the xap
                string xapPath = HttpContext.Current.Server.MapPath("~/ClientBin/" + this.Xap);
                DateTime xapCreationDate = System.IO.File.GetLastWriteTime(xapPath);

                //Use it to generate a version number
                long version = xapCreationDate.Ticks % 99999;

                //Add it to the xap path to force a download if the xap has been changed
                return this.Xap + "?v=" + version.ToString();
            }
        }

        //[Personalizable(PersonalizationScope.User), WebBrowsable(true)]
        //public eDisplayOption Display
        //{
        //    get { return _displayOption; }
        //    set { _displayOption = value; DisplayChart(); }
        //}

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

        public string SeriesData
        {
            get { return ViewState["__seriesdata"] == null ? "" : (string)ViewState["__seriesdata"]; }
            set { ViewState["__seriesdata"] = value; }
        }
    

        protected void Page_Load(object sender, EventArgs e)
        {
            //DisplayChart();
        }

        
    }
}
