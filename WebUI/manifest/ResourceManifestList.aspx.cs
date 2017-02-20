using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Telerik.Web.UI;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.manifest
{
    public partial class ResourceManifestList : Orchestrator.Base.BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdResourceManifestList.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdResourceManifestList_NeedDataSource);
            this.grdResourceManifestList.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdResourceManifestList_ItemDataBound);

            this.btnGetManifests.Click += new EventHandler(btnGetManifests_Click);
            
            this.cvDateRange.ServerValidate += new ServerValidateEventHandler(cvDateRange_ServerValidate);
        }

        void btnPrintSelected_Click(object sender, EventArgs e)
        {
            
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
            {
                Session[Orchestrator.Globals.Constants.ManifestListFromDate] = dteStartDate.SelectedDate;
                Session[Orchestrator.Globals.Constants.ManifestListToDate] = dteEndDate.SelectedDate;
                
                grdResourceManifestList.Rebind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session[Orchestrator.Globals.Constants.ManifestListFromDate] == null)
                    Session[Orchestrator.Globals.Constants.ManifestListFromDate] = DateTime.Today.AddDays(-5);

                if (Session[Orchestrator.Globals.Constants.ManifestListToDate] == null)
                    Session[Orchestrator.Globals.Constants.ManifestListToDate] = DateTime.Today.AddDays(1).Add(new TimeSpan(23, 59, 59));

                dteStartDate.SelectedDate = (DateTime)Session[Orchestrator.Globals.Constants.ManifestListFromDate];
                dteEndDate.SelectedDate = (DateTime)Session[Orchestrator.Globals.Constants.ManifestListToDate];
            }

            lblTitle.Text = string.Format("(for period {0} to {1})", ((DateTime)dteStartDate.SelectedDate).ToString("dd/MM/yy"), ((DateTime)dteEndDate.SelectedDate).ToString("dd/MM/yy"));
        }

        void grdResourceManifestList_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                Telerik.Web.UI.GridItem item = e.Item as Telerik.Web.UI.GridItem;

                string resourceManifestId = ((System.Data.DataRowView)e.Item.DataItem)["ResourceManifestId"].ToString();
                
                // Manifest numbers will be a minimum of 3 digits long.
                string formattedResourceManifestId = resourceManifestId;
                if (formattedResourceManifestId.Length == 1)
                    formattedResourceManifestId = string.Concat("00", formattedResourceManifestId);
                else if (formattedResourceManifestId.Length == 2)
                    formattedResourceManifestId = string.Concat("0", formattedResourceManifestId);

                HyperLink hypOrderManifestId = (HyperLink)item.FindControl("hypResourceManifestId");
                // depending if there is a subby on this manifest use a different link

                if (((System.Data.DataRowView)e.Item.DataItem)["SubContractorID"] != DBNull.Value)
                {
                    hypOrderManifestId.NavigateUrl = "SubbyResourceManifest.aspx?rmID=" + resourceManifestId;
                }
                else
                {
                    hypOrderManifestId.NavigateUrl = "ResourceManifest.aspx?rmID=" + resourceManifestId;
                }
                
                hypOrderManifestId.Text = formattedResourceManifestId;

                string scannedFormId = ((System.Data.DataRowView)e.Item.DataItem)["ScannedFormId"].ToString();

                HyperLink viewScanLink = (HyperLink)item.FindControl("hypViewScan");
                HtmlAnchor scanLink = (HtmlAnchor)item.FindControl("hypScan");

                if (String.IsNullOrEmpty(scannedFormId))
                {
                    scanLink.InnerText = "Scan";
                    scanLink.HRef = "javascript:ScanManifest(" + resourceManifestId + ");";
                    viewScanLink.Visible = false;
                }
                else
                {
                    string scannedFormPDF = ((System.Data.DataRowView)e.Item.DataItem)["ScannedFormPDF"].ToString();
                    scanLink.InnerText = "Re-Scan";
                    scanLink.HRef = "javascript:ScanExistingManifest(" + resourceManifestId + "," + scannedFormId + ");";
                    viewScanLink.Visible = true;
                    viewScanLink.NavigateUrl = scannedFormPDF;
                    viewScanLink.Text = "View |";
                }
                
                Label lblManifestDate = (Label)item.FindControl("lblManifestDate");
                lblManifestDate.Text = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["ManifestDate"].ToString()).ToShortDateString();

            }
        }

        void grdResourceManifestList_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            DataSet dsResourceManifest = facResourceManifest.GetResourceManifestList((DateTime)dteStartDate.SelectedDate, (DateTime)dteEndDate.SelectedDate);
            this.grdResourceManifestList.DataSource = dsResourceManifest;
        }
    }
}
