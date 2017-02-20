using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.FuelSurcharge
{

    public partial class FuelSurchargeList : Orchestrator.Base.BasePage
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdFuelSurcharge.NeedDataSource += grdFuelSurcharge_NeedDataSource;

            if (!Page.User.IsInRole(((int)Orchestrator.eUserRole.Pricing).ToString()))
            {
                if (!Page.User.IsInRole(((int)Orchestrator.eUserRole.SystemAdministrator).ToString()))
                {
                    this.pnlUserAuthorised.Visible = false;
                    this.pnlUserNotAuthorised.Visible = true;
                }
            }
            else
            {
                this.pnlUserAuthorised.Visible = true;
                this.pnlUserNotAuthorised.Visible = false;
                this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
                this.dlgAddFuelSurcharge.DialogCallBack += new EventHandler(dlgAddFuelSurcharge_DialogCallBack);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.btnAddFuelSurcharge.Attributes.Add("onclick", this.dlgAddFuelSurcharge.GetOpenDialogScript());
            }
        }

        private void grdFuelSurcharge_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var ctx = new Orchestrator.EF.DataContext())
            {
                this.grdFuelSurcharge.DataSource = ctx.FuelSurchargeSet.ToList();
            }
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            this.grdFuelSurcharge.Rebind();
        }

        void dlgAddFuelSurcharge_DialogCallBack(object sender, EventArgs e)
        {
            // Refresh the list.
            if (this.dlgAddFuelSurcharge.ReturnValue.ToLower() == "refresh")
                this.grdFuelSurcharge.Rebind();
        }

    }

}
