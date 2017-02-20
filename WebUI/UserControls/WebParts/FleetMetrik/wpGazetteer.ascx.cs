using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.UserControls.WebParts.FleetMetrik
{
    public partial class wpGazetteer : System.Web.UI.UserControl, IWebPart
    {

        #region Public Properties
        public string DataUrl { get; set; }

        public string Title { get; set; }

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }
        #endregion

        public wpGazetteer()
        {
            this.Title = "Gazetteer";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataUrl = string.Format("{0}/api/gpsPosition",
            ConfigurationManager.AppSettings["ApiBaseUrl"]);
        }
    }
}