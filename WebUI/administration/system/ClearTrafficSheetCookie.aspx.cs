using System;
using System.Web;
using System.Web.UI;

namespace Orchestrator.WebUI.administration.system
{
    public partial class ClearTrafficSheetCookie : Orchestrator.Base.BasePage
    {

        private const string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnContinue.Click += new EventHandler(btnContinue_Click);
        }

        #endregion

        #region Event Handlers

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            if (Request.Cookies[this.CookieSessionID] != null)
            {
                Response.Cookies[this.CookieSessionID].Values[C_TRAFFIC_SHEET_XML] = string.Empty;
                lblDone.Visible = true;
            }

            btnContinue.Visible = false;
        }

        #endregion
    }
}