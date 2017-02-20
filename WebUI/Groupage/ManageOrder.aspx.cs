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

using System.Collections.Generic;

using Orchestrator;
using Orchestrator.Entities;

namespace Orchestrator.WebUI.Groupage
{
    public partial class ManageOrder : Orchestrator.Base.BasePage
    {
        #region Properties

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        #endregion

        #region OnInit/Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            // if this is not an Orchestrator User then we need to restrict the points that can be selected.

            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["oid"]))
                    this.Master.WizardTitle = "Add Order";
                else
                    this.Master.WizardTitle = "Update Order";
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            bool openInWizardMode = false;
            if (bool.TryParse(Request.QueryString["wiz"], out openInWizardMode) && openInWizardMode)
            {
                // The page should open in "wizard" mode, change the master page being used.
                // Use the wizard master page instead.
                this.MasterPageFile = "~/WizardMasterPage.Master";
            }
        }

        #endregion
    }
}