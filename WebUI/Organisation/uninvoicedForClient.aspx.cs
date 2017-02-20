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
using Telerik.Web.UI;
using System.Data.SqlTypes;

namespace Orchestrator.WebUI.Organisation
{
    public partial class Organisation_uninvoicedForClient : Orchestrator.Base.BasePage
    {
        private const string c_AllUninvoicedWorkForOrganisation_VS = "vs_AllUninvoicedWorkForOrganisation";

        #region Properties

        public DataSet AllUninvoicedWorkForOrganisation
        {
            get
            {
                if (ViewState[c_AllUninvoicedWorkForOrganisation_VS] == null)
                {
                    int identityID = 0;
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    int.TryParse(cboClient.SelectedValue, out identityID);

                    if (identityID > 0)
                        ViewState[c_AllUninvoicedWorkForOrganisation_VS] = facOrg.GetAllUninvoicedWorkForOrganisation(identityID, this.rdiStartDate.SelectedDate.Value, this.rdiEndDate.SelectedDate.Value, cboSearchAgainstWorker.Items[0].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[1].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[2].Selected || cboSearchAgainstWorker.Items[3].Selected);
                    else
                        ViewState[c_AllUninvoicedWorkForOrganisation_VS] = null;
                }

                return (DataSet)ViewState[c_AllUninvoicedWorkForOrganisation_VS];
            }
            set { ViewState[c_AllUninvoicedWorkForOrganisation_VS] = value; }
        }

        #endregion

        #region Page Setup

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            
            this.repBusinessType.ItemDataBound += new RepeaterItemEventHandler(repBusinessType_ItemDataBound);

            this.grdNormal.NeedDataSource += new GridNeedDataSourceEventHandler(grdNormal_NeedDataSource);
            this.grdNormal.ItemDataBound += new GridItemEventHandler(grdNormal_ItemDataBound);
            
            this.grdSummary.NeedDataSource += new GridNeedDataSourceEventHandler(grdSummary_NeedDataSource);
            this.grdSummary.ItemDataBound += new GridItemEventHandler(grdSummary_ItemDataBound);
            
            this.grdUninvoiceExtras.NeedDataSource += new GridNeedDataSourceEventHandler(grdUninvoiceExtras_NeedDataSource);
            this.grdUninvoiceExtras.ItemDataBound += new GridItemEventHandler(grdUninvoiceExtras_ItemDataBound);
        }

        private void ConfigureDisplay()
        {
            DateTime startDate = DateTime.Today.AddMonths(-1);
            System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar();
            this.rdiStartDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, 01);
            this.rdiEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, cal.GetDaysInMonth(startDate.Year, startDate.Month));
        }

        #endregion

        #region ComboBox

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClient.DataSource = boundResults;
            cboClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region Buttons

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();
            int identityID = 0;
            int.TryParse(cboClient.SelectedValue, out identityID);

            AllUninvoicedWorkForOrganisation = facOrg.GetAllUninvoicedWorkForOrganisation(identityID, this.rdiStartDate.SelectedDate.Value, this.rdiEndDate.SelectedDate.Value, cboSearchAgainstWorker.Items[0].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[1].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[2].Selected || cboSearchAgainstWorker.Items[3].Selected);

            this.grdUninvoiceExtras.Rebind();
            BindRepeater();
        }

        #endregion

        #region Grids

        private void BindRepeater()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessType = null;

            dsBusinessType = facBusinessType.GetAll();

            repBusinessType.DataSource = dsBusinessType;
            repBusinessType.DataBind();
            grdNormal.Rebind();
            grdSummary.Rebind();
        }

        void repBusinessType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTitle = (Label)e.Item.FindControl("lblTitle");
                HiddenField hidBusinessTypeID = (HiddenField)e.Item.FindControl("hidBusinessTypeID");

                hidBusinessTypeID.Value = ((System.Data.DataRowView)(e.Item.DataItem)).Row["BusinessTypeID"].ToString();
                lblTitle.Text = "Groupage " + ((System.Data.DataRowView)(e.Item.DataItem)).Row["Description"].ToString();
            }
        }

        protected void grd_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Telerik.Web.UI.RadGrid grdBusinessType = (Telerik.Web.UI.RadGrid)source;

            HiddenField hidBusinessTypeID = (HiddenField)grdBusinessType.Parent.FindControl("hidBusinessTypeID");
            HtmlTableRow rowTitle = (HtmlTableRow)grdBusinessType.Parent.FindControl("rowTitle");
            HtmlTableRow rowGrid = (HtmlTableRow)grdBusinessType.Parent.FindControl("rowGrid");

            if (hidBusinessTypeID != null && grdBusinessType != null && AllUninvoicedWorkForOrganisation != null)
            {
                rowTitle.Style["display"] = "";
                rowGrid.Style["display"] = "";

                string businessTypeID = string.Empty;

                if (hidBusinessTypeID.Value == string.Empty)
                {
                    businessTypeID = ((System.Data.DataRowView)((RepeaterItem)grdBusinessType.Parent.Parent.Parent).DataItem).Row["BusinessTypeID"].ToString();
                    hidBusinessTypeID.Value = businessTypeID;
                }
                else
                    businessTypeID = hidBusinessTypeID.Value;

                AllUninvoicedWorkForOrganisation.Tables[0].DefaultView.RowFilter = "BusinessTypeID = " + businessTypeID;
                grdBusinessType.DataSource = AllUninvoicedWorkForOrganisation.Tables[0].DefaultView;

                if (AllUninvoicedWorkForOrganisation.Tables[0].DefaultView.Count < 1)
                {
                    rowTitle.Style["display"] = "none";
                    rowGrid.Style["display"] = "none";
                }
            }
        }

        protected void grd_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFooterItem)
            {
                Telerik.Web.UI.RadGrid grdBusinessType = (Telerik.Web.UI.RadGrid)sender;
                HiddenField hidBusinessTypeID = (HiddenField)grdBusinessType.Parent.FindControl("hidBusinessTypeID");

                AllUninvoicedWorkForOrganisation.Tables[2].DefaultView.RowFilter = "BusinessTypeID = " + hidBusinessTypeID.Value;
                Label lblPalletTotal = (Label)e.Item.FindControl("lblTotalUninvoiced");
                
                if (AllUninvoicedWorkForOrganisation.Tables[2].DefaultView.Count > 0)
                {
                    Decimal totalValue = Convert.ToDecimal(AllUninvoicedWorkForOrganisation.Tables[2].DefaultView[0].Row["Total Rate"]);
                    lblPalletTotal.Text = totalValue.ToString("C");
                }
                else
                    lblPalletTotal.Text = 0d.ToString("C");
            }
            else if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces");
                if (spnPalletSpaces != null)
                    switch (spnPalletSpaces.InnerText)
                    {
                        case "0.25":
                            spnPalletSpaces.InnerText = "¼";
                            break;

                        case "0.5":
                            spnPalletSpaces.InnerText = "½";
                            break;

                        default:
                            break;
                    }

                int orderID = (int)drv["OrderID"];


                SetupHasPodLink(e, drv);
                

                Repeater repReferences = (Repeater)e.Item.FindControl("repReferences");
                repReferences.DataSource =
                    drv.Row.Table.DataSet.Tables[3].Select("OrderID = " + orderID);
                repReferences.DataBind();
                if ((int)drv["jobStateID"] == 1)
                    e.Item.BackColor = System.Drawing.Color.White;
                if ((int)drv["jobStateID"] == 2)
                    e.Item.BackColor = System.Drawing.Color.FromArgb(204, 255, 204);
                if ((int)drv["jobStateID"] == 3)
                    e.Item.BackColor = System.Drawing.Color.FromArgb(153, 255, 153);
                if ((int)drv["jobStateID"] == 4)
                    e.Item.BackColor = System.Drawing.Color.LightBlue;
                if ((int)drv["jobStateID"] == 5)
                    e.Item.BackColor = System.Drawing.Color.MistyRose;
                if ((int)drv["jobStateID"] == 6)
                    e.Item.BackColor = System.Drawing.Color.PaleVioletRed;
                if ((int)drv["jobStateID"] == 7)
                    e.Item.BackColor = System.Drawing.Color.Yellow;
                if ((int)drv["jobStateID"] == 8)
                    e.Item.BackColor = System.Drawing.Color.Gold;
                if ((int)drv["jobStateID"] == 9)
                    e.Item.BackColor = System.Drawing.Color.Khaki;
            }
        }

        void grdNormal_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["rcbID"]) && AllUninvoicedWorkForOrganisation != null)
            {
                AllUninvoicedWorkForOrganisation.Tables[0].DefaultView.RowFilter = "BusinessTypeID = 0";

                if (AllUninvoicedWorkForOrganisation.Tables[0].DefaultView.Count > 0)
                {
                    grdNormal.DataSource = AllUninvoicedWorkForOrganisation.Tables[0].DefaultView;
                    tblNormal.Style["display"] = "";
                }
                else
                    tblNormal.Style["display"] = "none";
            }
        }

        void grdNormal_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView) e.Item.DataItem;
                int jobID = (int) drv["JobID"];
                string deliveryOrderNumber = (string) drv["DeliveryOrderNumber"];

                DataRow[] otherRowsForThisJob =
                    AllUninvoicedWorkForOrganisation.Tables[0].Select("BusinessTypeID = 0 AND JobID = " +
                                                                      jobID.ToString() + " AND DeliveryOrderNumber <> '" +
                                                                      deliveryOrderNumber + "'");

                SetupHasPodLink(e, drv);

                if (otherRowsForThisJob.Length > 0)
                {
                    // This job's rate information may already have been displayed in the grid.
                    GridDataItem[] gdi = new GridDataItem[grdNormal.Items.Count];
                    grdNormal.Items.CopyTo(gdi, 0);
                    List<GridDataItem> normalItems = new List<GridDataItem>(gdi);
                    if (normalItems.Find(delegate(GridDataItem testItem)
                    {
                        bool match = (int)((DataRowView)testItem.DataItem)["JobID"] == jobID;
                        return match;
                    }
                    ) != null)
                    {
                        HtmlGenericControl spnCharge = (HtmlGenericControl)e.Item.FindControl("spnCharge");
                        spnCharge.Visible = false;
                    }   
                    else
                    {

                        Repeater repReferences = (Repeater)e.Item.FindControl("repReferences");
                        repReferences.DataSource =
                            drv.Row.Table.DataSet.Tables[3].Select("JobID = " + jobID);
                        repReferences.DataBind();                        
                    }
                }
            }
            else if (e.Item is GridFooterItem)
            {
                AllUninvoicedWorkForOrganisation.Tables[2].DefaultView.RowFilter = "BusinessTypeID = 0";
                Label lblPalletTotal = (Label)e.Item.FindControl("lblTotalUninvoiced");

                if (AllUninvoicedWorkForOrganisation.Tables[2].DefaultView.Count > 0)
                {
                    Decimal totalValue = Convert.ToDecimal(AllUninvoicedWorkForOrganisation.Tables[2].DefaultView[0].Row["Total Rate"]);
                    lblPalletTotal.Text = totalValue.ToString("C");
                }
                else
                    lblPalletTotal.Text = 0d.ToString("C");
            }
        }

        private static void SetupHasPodLink(GridItemEventArgs e, DataRowView drv)
        {
            HyperLink lnkPOD = (HyperLink)e.Item.FindControl("lnkPOD");
            bool hasPod = (bool)drv["HasPOD"];

            if (hasPod)
            {
                lnkPOD.ForeColor = System.Drawing.Color.Blue;
                lnkPOD.NavigateUrl = drv.Row["ScannedFormPDF"].ToString().Trim();
                lnkPOD.Text = "Yes";
            }
            else
            {
                var orderStatus = (eOrderStatus)drv["OrderStatusID"];

                if (orderStatus == eOrderStatus.Delivered || orderStatus == eOrderStatus.Invoiced)
                {
                    int orderID = (int)drv["OrderID"];
                    lnkPOD.Text = "No";
                    lnkPOD.ForeColor = System.Drawing.Color.Blue;
                    lnkPOD.NavigateUrl = @"javascript:OpenPODWindow(" + orderID + ")";
                }
                else
                {
                    lnkPOD.Text = "N/A";
                    lnkPOD.ToolTip = "Not Delivered";
                    lnkPOD.Style.Add("text-decoration", "none");
                }
            }
        }

        void grdSummary_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["rcbID"]) && AllUninvoicedWorkForOrganisation != null)
            {
                if (AllUninvoicedWorkForOrganisation.Tables[1].DefaultView.Count > 0)
                {
                    grdSummary.DataSource = AllUninvoicedWorkForOrganisation.Tables[1].DefaultView;
                    tblSummary.Style["display"] = "";
                }
                else
                    tblSummary.Style["display"] = "none";
            }
        }

        void grdSummary_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFooterItem)
            {
                Label lblTotalCountOfJobs = (Label)e.Item.FindControl("lblTotalCountOfJobs");
                Label lblTotalCountOfExtras = (Label)e.Item.FindControl("lblTotalCountOfExtras");
                Label lblTotalRate = (Label)e.Item.FindControl("lblTotalRate");

                int totalCountOfJobs = 0;
                int totalCountOfExtras = 0;
                decimal totalRate = 0;

                foreach (DataRow row in AllUninvoicedWorkForOrganisation.Tables[1].Rows)
                {
                    totalCountOfJobs += (int)row["CountOfJobs"];
                    totalRate += (decimal)row["Total Charge Amount"];
                    totalCountOfExtras += (int)row["CountOfExtras"];
                }

                lblTotalCountOfJobs.Text = totalCountOfJobs.ToString();
                lblTotalRate.Text = totalRate.ToString("C");
                lblTotalCountOfExtras.Text = totalCountOfExtras.ToString();
            }
        }

        void grdUninvoiceExtras_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFooterItem)
            {
                decimal totalExtras = decimal.Zero;

                Label lbluninvocieExtraTotal = (Label)e.Item.FindControl("lblTotalUninvoicedExtras");
                DataSet dsUninvoicedExtras = (DataSet)grdUninvoiceExtras.DataSource;
                foreach (DataRow row in dsUninvoicedExtras.Tables[0].Rows)
                {
                    totalExtras += (decimal)row["ForeignAmount"];
                }
                lbluninvocieExtraTotal.Text = totalExtras.ToString("C");
            }
        }

        void grdUninvoiceExtras_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (AllUninvoicedWorkForOrganisation != null)
            {
                this.divUnInvoicedExtras.Style["Display"] = String.Empty;
                Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
                int identityID = 0;
                int.TryParse(cboClient.SelectedValue, out identityID);

                DataSet dsUnInvoiceExtras = facInvoiceExtra.GetAllUninvoicedExtras(identityID, this.rdiStartDate.SelectedDate.Value, this.rdiEndDate.SelectedDate.Value);
                this.grdUninvoiceExtras.DataSource = dsUnInvoiceExtras;
            }
        }

        #endregion
    }
}
