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

using System.Globalization;
using System.Data.SqlTypes;

namespace Orchestrator.WebUI.Groupage
{
    public partial class autorunstart : Orchestrator.Base.BasePage
    {
        private string userName;

        public bool IsFromDash
        {
            get { return ViewState["vw_isFromDash"] == null ? false : bool.Parse(ViewState["vw_isFromDash"].ToString()); }
            set { ViewState["vw_isFromDash"] = value; }
        }

        public int m_IdentityId
        {
            get { return ViewState["vw_IdentityId"] == null ? -1 : int.Parse(ViewState["vw_IdentityId"].ToString()); }
            set { ViewState["vw_IdentityId"] = value; }
        }

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnCreateBatch.Click += new EventHandler(btnCreateBatch_Click);
            this.Button1.Click += new EventHandler(btnCreateBatch_Click);
            
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

        #endregion

        #region Grid View Events

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        #endregion

        #region Private Methods

        private void ConfigureDisplay()
        {
            string MinDate = System.DateTime.Today.ToString();

            if (!string.IsNullOrEmpty(Request.QueryString["IdentityId"]))
            {
                int IdentityID = 0;
                int.TryParse(Request.QueryString["IdentityID"].ToString(), out IdentityID);
                m_IdentityId = IdentityID;
                IsFromDash = true;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["MinDate"]))
            {
                MinDate = Request.QueryString["MinDate"];
                IsFromDash = true;
            }

            if (IsFromDash)
            {
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboClient.SelectedValue = m_IdentityId.ToString();
                cboClient.Text = name;

                DateTime startDate = new DateTime();
                DateTime endDate = System.DateTime.Today;
                DateTime.TryParse(MinDate, out startDate);

                rdiStartDate.SelectedDate = startDate;
                rdiEndDate.SelectedDate = endDate;

                //Unselect NORM and select BOTH
                rdoListInvoiceType.Items[0].Selected = false;
                rdoListInvoiceType.Items[2].Selected = true;
            }

            // This has been removed re TFS Issue : #22431
            //else
            //{
            //    DateTime startDate = DateTime.Today.AddMonths(-1);
            //    System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar();

            //    this.rdiStartDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, 01);
            //    this.rdiEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, cal.GetDaysInMonth(startDate.Year, startDate.Month));
            //}
        }

        private DataSet LoadBatch()
        {
            int batchID = int.Parse(Request.QueryString["bID"]);
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            DataSet dsBatch = facPreInvoice.GetOrdersForBatch(batchID);
            rdiInvoiceDate.SelectedDate = (DateTime)dsBatch.Tables[0].Rows[0]["InvoiceDate"];
            return dsBatch;
        }

        private DataSet GetReadyToInvoiceForDates()
        {
            int clientID = 0;

            if (!IsFromDash)
                int.TryParse(cboClient.SelectedValue, out clientID);
            else
                clientID = m_IdentityId;

            DataSet ds = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            ds = facOrder.GetOrdersReadyToInvoice(rdiStartDate.SelectedDate, rdiEndDate.SelectedDate, rdoListInvoiceType.Items[0].Selected || rdoListInvoiceType.Items[2].Selected, rdoListInvoiceType.Items[1].Selected || rdoListInvoiceType.Items[2].Selected, clientID);
            IsFromDash = false;

            return ds;
        }

        #endregion

        #region Event Handlers

        #region Buttons

        void btnRefresh_Click(object sender, EventArgs e)
        {
            DataSet ds = null;
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                ds = null;
            else if (!string.IsNullOrEmpty(Request.QueryString["bID"]))
                ds = LoadBatch();
            else
                ds = GetReadyToInvoiceForDates();

            this.grdOrders.DataSource = ds;
            this.grdOrders.DataBind();
        }

        void btnCreateBatch_Click(object sender, EventArgs e)
        {
            List<int> selectedOrderIDs = new List<int>();
            foreach (GridItem row in grdOrders.Items)
            {
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    CheckBox chk = row.FindControl("chkSelectOrder") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                        if (!selectedOrderIDs.Contains(orderID))
                            selectedOrderIDs.Add(orderID);
                    }
                }
            }

            if (selectedOrderIDs.Count > 0 && Page.IsValid)
            {
                int batchID;
                Orchestrator.Facade.IPreInvoice facPreInvoice = new Orchestrator.Facade.PreInvoice();
                userName = Page.User.Identity.Name;
                if (IsUpdate())
                {
                    batchID = int.Parse(Request.QueryString["bID"]);
                    facPreInvoice.UpdateBatch(batchID, rdiInvoiceDate.SelectedDate.Value, selectedOrderIDs, userName);
                }
                else
                {
                    batchID = facPreInvoice.CreateBatch(rdiInvoiceDate.SelectedDate.Value, selectedOrderIDs, userName);
                   
                }
                Response.Redirect("autorunconfirmation.aspx?bID=" + batchID);
            }
        }

        #endregion

        #region Grid

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectAll");
                if (chk != null)
                    chk.Attributes.Add("onclick","javascript:HandleGridSelection();");
            }

            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView) e.Item.DataItem;

                if (e.Item.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    CheckBox chkSelectOrder = (CheckBox)e.Item.FindControl("chkSelectOrder");
                    Label lblCharge = e.Item.FindControl("lblCharge") as Label;

                    chkSelectOrder.Attributes.Add("onclick",
                                                  string.Format(
                                                      "javascript:HandleSelection(event, this, {0});",
                                                      drv["OrderGroupID"]));

                    //int orderGroupID = (int) drv["OrderGroupID"];
                    //if (orderGroupID > 0)
                    //{
                    //    // This order's rate information may already have been displayed in the grid.
                    //    GridDataItem[] gdi = new GridDataItem[grdOrders.Items.Count];
                    //    grdOrders.Items.CopyTo(gdi, 0);
                    //    List<GridDataItem> normalItems = new List<GridDataItem>(gdi);
                    //    if (normalItems.Find(delegate(GridDataItem testItem)
                    //                             {
                    //                                 bool match =
                    //                                     (int) ((DataRowView) testItem.DataItem)["OrderGroupID"] ==
                    //                                     orderGroupID;
                    //                                 return match;
                    //                             }
                    //            ) != null)
                    //    {
                    //        if (lblCharge != null)
                    //            lblCharge.Visible = false;
                    //    }
                    //}

                    if (lblCharge != null)
                    {
                        CultureInfo culture = new CultureInfo((int)drv["LCID"]);
                        lblCharge.Text = ((decimal)drv["ForeignRate"]).ToString("C", culture);
                    }
                }
            }
        }

        #endregion

        #region Drop Down Boxes

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

        private bool IsUpdate()
        {
            bool retval = !string.IsNullOrEmpty(Request.QueryString["bID"]);
            return retval;
        }

        #endregion
    }

}