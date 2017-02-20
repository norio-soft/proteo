using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Groupage
{
    public partial class AddOrder : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            // if this is not an Orchestrator User then we need to restrict the points that can be selected.
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["hm"] == "1")
                {
                    ((Masterpage_Tableless)this.Master).HideMenu = true;
                }
                if (!Page.IsPostBack)
                {
                    if (string.IsNullOrEmpty(Request["oid"]))
                        lblHeader.Text = "Add Order";
                    else
                        lblHeader.Text = "Update Order";
                }
            }
        }

        #region Properties

        //protected override PageStatePersister PageStatePersister
        //{
        //    get
        //    {
        //        return new HiddenFieldPageStatePersister(this);
        //    }
        //}

        #endregion
    }
}
