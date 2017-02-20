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

using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Pallet
{
    public partial class ListPalletTypes : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (!IsPostBack)
                BindPalletTypes();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAddPalletType.Click += new EventHandler(btnAddPalletType_Click);
        }

        private void BindPalletTypes()
        {
            gvPalletTypes.DataSource = Facade.PalletType.GetAllPalletTypes();
            gvPalletTypes.DataBind();
        }

        #region Event Handlers

        #region Button Events

        void btnAddPalletType_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddUpdatePalletType.aspx");
        }

        #endregion

        #endregion
    }
}