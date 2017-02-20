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
using System.Globalization;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing.Approval
{
    public partial class ReviewableGroups : Orchestrator.Base.BasePage
    {
        private Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // add the english culture to the list.
            this.cultures.Add(2057, new CultureInfo(2057));

            gvGroups.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvGroups_NeedDataSource);
            gvGroups.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(gvGroups_ItemCommand);
            gvGroups.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(gvGroups_ItemDataBound);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }

        #region Event Handlers

        #region Button Events

        void btnRefresh_Click(object sender, EventArgs e)
        {
            gvGroups.Rebind();
        }

        #endregion

        #region Grid Events

        void gvGroups_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;

                int LCID = 2057;
                if (drv["LCID"] != DBNull.Value && Convert.ToInt32(drv["LCID"]) != -1)
                    LCID = Convert.ToInt32(drv["LCID"]);

                if (!this.cultures.ContainsKey(LCID))
                    this.cultures.Add(LCID, new CultureInfo(LCID));

                // add formatted currency values based on culture.
                Label netAmountCurrencyLabel = (Label)e.Item.FindControl("netAmountCurrencyLabel");
                netAmountCurrencyLabel.Text = Convert.ToDecimal(drv["ForeignNetAmount"]).ToString("C", this.cultures[LCID]);

                Label extraAmountCurrencyLabel = (Label)e.Item.FindControl("extraAmountCurrencyLabel");
                extraAmountCurrencyLabel.Text = Convert.ToDecimal(drv["ForeignExtraAmount"]).ToString("C", this.cultures[LCID]);

                Label fuelSurchargeAmountCurrencyLabel = (Label)e.Item.FindControl("fuelSurchargeAmountCurrencyLabel");
                fuelSurchargeAmountCurrencyLabel.Text = Convert.ToDecimal(drv["ForeignFuelSurchargeAmount"]).ToString("C", this.cultures[LCID]);

                Label taxAmountCurrencyLabel = (Label)e.Item.FindControl("taxAmountCurrencyLabel");
                taxAmountCurrencyLabel.Text = Convert.ToDecimal(drv["ForeignTaxAmount"]).ToString("C", this.cultures[LCID]);

                Label grandTotalAmountCurrencyLabel = (Label)e.Item.FindControl("grandTotalAmountCurrencyLabel");
                grandTotalAmountCurrencyLabel.Text = Convert.ToDecimal(drv["ForeignTotalAmount"]).ToString("C", this.cultures[LCID]);
            }
        }

        void gvGroups_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "approvegroup":
                    Response.Redirect("InvoiceReview.aspx?wid=" + gvGroups.MasterTableView.DataKeyValues[e.Item.ItemIndex][e.CommandArgument.ToString()]);
                    break;
            }
        }

        void gvGroups_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IWorkflowPreInvoice facPreInvoice = new Facade.PreInvoice();
            gvGroups.DataSource = facPreInvoice.GetAll();
        }

        #endregion

        #endregion
    }
}