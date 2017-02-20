using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports
{
    public partial class OrderBasedManifestList : Orchestrator.Base.BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdManifestList.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdManifestList_NeedDataSource);
            this.grdManifestList.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdManifestList_ItemDataBound);
            this.cvDateRange.ServerValidate += new ServerValidateEventHandler(cvDateRange_ServerValidate);
            this.btnGetManifests.Click +=new EventHandler(btnGetManifests_Click);
        }

        void cvDateRange_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (((DateTime)dteEndDate.SelectedDate).Subtract((DateTime)dteStartDate.SelectedDate).Days > 100)
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        void btnGetManifests_Click(object sender, EventArgs e)
        {
            // Get the manifests for the specified date range
            if (Page.IsValid)
                grdManifestList.Rebind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dteStartDate.SelectedDate = DateTime.Today;
                dteEndDate.SelectedDate = DateTime.Today.AddDays(1).Add(new TimeSpan(23, 59, 59));
            }

            lblTitle.Text = string.Format("(for period {0} to {1})", ((DateTime)dteStartDate.SelectedDate).ToString("dd/MM/yy"), ((DateTime)dteEndDate.SelectedDate).ToString("dd/MM/yy"));
        }

        void grdManifestList_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                GridItem item = e.Item as GridItem;

                HyperLink hypOrderManifestId = (HyperLink)item.FindControl("hypOrderManifestId");
                hypOrderManifestId.NavigateUrl = "OrderBasedManifest.aspx?omID=" + ((System.Data.DataRowView)e.Item.DataItem)["OrderManifestId"].ToString();

                // Manifest numbers will be a minimum of 3 digits long.
                string formattedOrderManifestId = ((System.Data.DataRowView)e.Item.DataItem)["OrderManifestId"].ToString();
                if (formattedOrderManifestId.Length == 1)
                    formattedOrderManifestId = string.Concat("00", formattedOrderManifestId);
                else if (formattedOrderManifestId.Length == 2)
                    formattedOrderManifestId = string.Concat("0", formattedOrderManifestId);
                
                hypOrderManifestId.Text = formattedOrderManifestId;

                Label lblManifestDate = (Label)item.FindControl("lblManifestDate");
                lblManifestDate.Text = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["ManifestDate"].ToString()).ToShortDateString();

                Label lblResource = (Label)item.FindControl("lblResource");
                string resourceName = string.Empty;
                string subcontractorName = string.Empty;

                resourceName = ((System.Data.DataRowView)e.Item.DataItem)["ResourceName"].ToString();
                subcontractorName = ((System.Data.DataRowView)e.Item.DataItem)["SubContractorName"].ToString();

                lblResource.Text = resourceName == string.Empty ? subcontractorName : resourceName;
            }
        }

        void grdManifestList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Facade.OrderManifest facOrderManifest = new Orchestrator.Facade.OrderManifest();
            DataSet dsOrderManifest = facOrderManifest.GetOrderManifestList((DateTime)dteStartDate.SelectedDate, (DateTime)dteEndDate.SelectedDate);
            this.grdManifestList.DataSource = dsOrderManifest;
        }
    }
}
