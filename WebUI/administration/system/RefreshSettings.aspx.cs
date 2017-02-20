using System;
using System.Web;
using System.Web.UI;
using Orchestrator.Globals;


namespace Orchestrator.WebUI.administration.system
{
    public partial class RefreshSettings : Orchestrator.Base.BasePage
    {
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

            Configuration.RefreshSettings();

            Orchestrator.Application.RefreshTypeMappings();

            lblDone.Visible = true;
            btnContinue.Visible = false;
        }

        #endregion
    }
}