using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

using System.Collections.Generic;

namespace Orchestrator.WebUI.Client
{
    public partial class ClientOrderList : Orchestrator.Base.BasePage
    {
        #region Page properties
        private eOrderStatus _orderStatus = eOrderStatus.Approved;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            GetClientId();


            if (!this.IsPostBack)
            {

                this.dteOrderFilterDateFrom.SelectedDate = DateTime.Now.AddDays(-14);
                this.dteOrderFilterDateTo.SelectedDate = DateTime.Now;
                LoadDropDowns();
                this.grdOrders.Rebind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //this.grdOrders.Init += new EventHandler(grdOrders_Init);
            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.grdOrders.UpdateCommand += new GridCommandEventHandler(grdOrders_UpdateCommand);
            this.grdOrders.ItemCreated += new GridItemEventHandler(grdOrders_ItemCreated);
            this.btnFilter.ServerClick += new EventHandler(btnFilter_ServerClick);
            this.btnPIL.Click += new EventHandler(btnPIL_Click);
            this.cboOrderStatus.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboOrderStatus_SelectedIndexChanged);
        }

        void btnPIL_Click(object sender, EventArgs e)
        {
            LoadPILReport();
        }

        void btnFilter_ServerClick(object sender, EventArgs e)
        {
            grdOrders.Rebind();
        }

        public void grdOrders_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = grdOrders.FilterMenu;
            int i = 0;

            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }

        void cboOrderStatus_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            // filter the list of orders by status
            this._orderStatus = (eOrderStatus)Enum.Parse(typeof(eOrderStatus), e.Text);
            this.grdOrders.Rebind();
        }

        void grdOrders_ItemCreated(object sender, GridItemEventArgs e)
        {
            // Set the width of column filter text boxes
            if (e.Item is GridFilteringItem)
            {
                // OrderId column
                GridFilteringItem filter = (GridFilteringItem)e.Item;
                TextBox txtbx = (TextBox)filter.Controls[2].Controls[0];
                txtbx.Width = Unit.Pixel(50);

                // Status column
                //TextBox txtbx2 = (TextBox)filter.Controls[3].Controls[0];
                //txtbx2.Width = Unit.Pixel(70);
            }
        }

        void grdOrders_UpdateCommand(object source, GridCommandEventArgs e)
        {

        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                Telerik.Web.UI.GridItem item = e.Item as Telerik.Web.UI.GridItem;

                int invoiceId = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["InvoiceId"].ToString());
                string invoicePdfLocation = ((System.Data.DataRowView)e.Item.DataItem)["InvoicePdfLocation"].ToString();
                int orderId = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["OrderId"].ToString());
                bool isPalletNetwork = (bool)(((System.Data.DataRowView)e.Item.DataItem)["IsPalletNetwork"]);
                bool isExported = !string.IsNullOrEmpty((((System.Data.DataRowView)e.Item.DataItem)["IntegrationReference"]).ToString());
                eOrderStatus orderStatus = (eOrderStatus)(Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["OrderStatusId"].ToString()));

                string collectionPointDescription = ((System.Data.DataRowView)e.Item.DataItem)["CollectionPointDescription"].ToString();
                string deliveryPointDescription = ((System.Data.DataRowView)e.Item.DataItem)["DeliveryPointDescription"].ToString();

                Label lblCollectFromPoint = (Label)item.FindControl("lblCollectFromPoint");
                lblCollectFromPoint.Text = collectionPointDescription;

                Label lblDeliverToPoint = (Label)item.FindControl("lblDeliverToPoint");
                lblDeliverToPoint.Text = deliveryPointDescription;

                int orderLcid = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["OrderLcid"].ToString());

                int orderGroupLcid = 0;
                if (((System.Data.DataRowView)e.Item.DataItem)["OrderGroupLcid"].ToString() != string.Empty)
                    orderGroupLcid = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["OrderGroupLcid"].ToString());

                System.Globalization.CultureInfo culture = null;

                if (orderGroupLcid > 0)
                    culture = new CultureInfo(orderGroupLcid);
                else
                    culture = new CultureInfo(orderLcid);

                // Rate:: formatted for current order culture.
                Label lblRate = (Label)item.FindControl("lblRate");
                if (((System.Data.DataRowView)e.Item.DataItem)["DisplayRate"].ToString() != string.Empty)
                    lblRate.Text = Convert.ToDecimal(((System.Data.DataRowView)e.Item.DataItem)["DisplayRate"]).ToString("C", culture);
                else
                    lblRate.Text = "&nbsp;";

                // Pod link
                Facade.POD facPod = new Orchestrator.Facade.POD();
                Orchestrator.Entities.POD scannedPOD = facPod.GetForOrderID(orderId);
                HyperLink hypPod = (HyperLink)item.FindControl("hypPod");

                if (scannedPOD != null)
                {
                    hypPod.Visible = true;
                    hypPod.Text = string.Format("POD:{0}", scannedPOD.PODId);
                    hypPod.NavigateUrl = scannedPOD.ScannedFormPDF.Trim();
                    hypPod.Target = "_blank";
                }
                else if (scannedPOD == null && (orderStatus == eOrderStatus.Delivered || orderStatus == eOrderStatus.Invoiced))
                {
                    hypPod.Visible = false;
                    hypPod.Text = "";
                    hypPod.NavigateUrl = "";
                }


                // Invoice link
                HyperLink hypInvoice = (HyperLink)e.Item.FindControl("hypInvoice");
                if (invoiceId <= 0)
                {
                    hypInvoice.Text = "";
                    hypInvoice.NavigateUrl = "";
                }
                else
                {
                    hypInvoice.NavigateUrl = Orchestrator.Globals.Configuration.WebServer + invoicePdfLocation;
                    hypInvoice.Text = string.Format("Invoice:{0}", invoiceId);
                    hypInvoice.Target = "_blank";
                }

                // order id link
                HyperLink hypOrderId = (HyperLink)e.Item.FindControl("hypOrderId");
                hypOrderId.Text = orderId.ToString();

                // If the order is unapproved, take the client to the update order page
                // otherwise take the user to the client order profile screen.
                if (orderStatus == eOrderStatus.Pending || orderStatus == eOrderStatus.Awaiting_Approval)
                {
                    hypOrderId.NavigateUrl = string.Format("ClientManageOrder.aspx?Oid={0}", orderId);
                }
                else
                {
                    // show the order profile link.
                    hypOrderId.NavigateUrl = string.Format("javascript:openResizableDialogWithScrollbars('ClientOrderProfile.aspx?wiz=true&Oid={0}',560,700);", orderId);
                    hypOrderId.Target = "";
                }

                //Order status
                Label lblOrderStatus = (Label)e.Item.FindControl("lblOrderStatus");
                string orderStatusText = string.Empty;
                switch (orderStatus)
                {
                    case eOrderStatus.Pending:
                        orderStatusText = "Pending";
                        break;
                    case eOrderStatus.Awaiting_Approval:
                        orderStatusText = "Unapproved";
                        break;
                    case eOrderStatus.Approved:
                        orderStatusText = "Approved";
                        break;
                    case eOrderStatus.Delivered:
                        orderStatusText = "Delivered";
                        break;
                    case eOrderStatus.Invoiced:
                        orderStatusText = "Invoiced";
                        break;
                    case eOrderStatus.Cancelled:
                        orderStatusText = "Cancelled";
                        break;
                    case eOrderStatus.Rejected:
                        orderStatusText = "Rejected";
                        break;
                    default:
                        break;
                }

                lblOrderStatus.Text = orderStatusText;

                if (Orchestrator.Globals.Configuration.PalletNetworkLabelID == eReportType.PalletNetworkLabel)
                {
                    if(isExported)
                        e.Item.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }

        private List<int> GetSelectedOrderIds()
        {
            List<int> orderIDs = new List<int>();

            //loop through grid and add all the orderids of orders that are checked to the orderids list.
            foreach (GridItem selectedOrders in this.grdOrders.SelectedItems)
            {
                string orderid = selectedOrders.OwnerTableView.DataKeyValues[selectedOrders.ItemIndex]["OrderID"].ToString();
                orderIDs.Add(Convert.ToInt32(orderid));
            }

            return orderIDs;
        }

        private int _clientIdentityId = 0;
        private void GetClientId()
        {
            Entities.CustomPrincipal cp = Page.User as Entities.CustomPrincipal;

            if (cp.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                Facade.IUser facUser = new Facade.User();
                SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal)Page.User).UserName);
                reader.Read();

                if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                {
                    int RelatedIdentityID = int.Parse(reader["RelatedIdentityId"].ToString());
                    _clientIdentityId = RelatedIdentityID;
                }
                else
                {
                    throw new ApplicationException("User is not a client user.");
                }
            }
        }


        private void LoadDropDowns()
        {
            cboOrderStatus.DataSource = Enum.GetNames(typeof(eOrderStatus));
            cboOrderStatus.DataBind();
            cboOrderStatus.FindItemByText(eOrderStatus.All.ToString()).Remove();
            RadComboBoxItem allItem =  new RadComboBoxItem("All","-1");
            allItem.Selected = true;
            cboOrderStatus.Items.Insert(0,allItem);
        }

        private void LoadPILReport()
        {

            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            string OrderIDs = string.Empty;
            bool isPalletNetwork = false;

            foreach (GridDataItem item in grdOrders.SelectedItems)
            {
                isPalletNetwork = (bool)item.OwnerTableView.DataKeyValues[item.ItemIndex]["IsPalletNetwork"];
                OrderIDs += item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString();
                OrderIDs += ",";
            }

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (OrderIDs.Length > 0)
            {
                #region Pop-up Report

                eReportType reportType = eReportType.PIL;
                dsPIL = facLoadOrder.GetPILData(OrderIDs.ToString());
                DataView dvPIL;

                if (isPalletNetwork)
                {
                    reportType = Orchestrator.Globals.Configuration.PalletNetworkLabelID;

                    //Need to duplicate the rows for the Pallteforce labels
                    dsPIL.Tables[0].Merge(dsPIL.Tables[0].Copy(), true);
                    dvPIL = new DataView(dsPIL.Tables[0], string.Empty, "OrderId, PalletCount", DataViewRowState.CurrentRows);
                }
                else
                {
                    dvPIL = new DataView(dsPIL.Tables[0]);
                }

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = reportType;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dvPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
                #endregion

            }
        } 


        private void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.grdOrders.DataSource = null;

            Facade.IOrder facOrder = new Facade.Order();

            DateTime dateFrom = dteOrderFilterDateFrom.SelectedDate.Value;
            DateTime dateTo = dteOrderFilterDateTo.SelectedDate.Value;
            this._orderStatus = (eOrderStatus)Enum.Parse(typeof(eOrderStatus),cboOrderStatus.SelectedValue);

            bool IsCollectionDate = rbCollection.Checked;

            dateFrom = dateFrom.AddHours(-dateFrom.Hour).AddMinutes(-dateFrom.Minute).AddSeconds(-dateFrom.Second);
            dateTo = dateTo.AddHours(-dateTo.Hour).AddMinutes(-dateTo.Minute).AddSeconds(-dateTo.Second);

            DataSet orderData = facOrder.GetOrdersForClientAndStatusByDates(_clientIdentityId, _orderStatus, IsCollectionDate, dateFrom, dateTo);
            orderData.Tables[0].Columns.Add(new DataColumn("DisplayRate"));

            GridColumn deliveringResourceColumn = this.grdOrders.Columns.FindByUniqueName("DeliveringResource");
            Facade.IAllocation facAllocation = new Facade.Allocation();
            if (facAllocation.IsAllocationEnabled)
            {
                if (deliveringResourceColumn != null)
                    deliveringResourceColumn.Visible = true;
            }
            else
                if (deliveringResourceColumn != null)
                    deliveringResourceColumn.Visible = false;

            // Iterate over the dataset to set the rates, specifically for grouped orders.
            DataRow previousRow = null;
            string previousOrderGroupId = string.Empty;
            foreach (DataRow row in orderData.Tables[0].Rows)
            {
                string orderGroupId = string.Empty;
                if (previousRow != null)
                {
                    if (row["OrderGroupId"].ToString() != "")
                    {
                        orderGroupId = row["OrderGroupId"].ToString();

                        // If the group is the same as the previous, blank the rate
                        if (previousOrderGroupId == orderGroupId)
                        {
                            row["DisplayRate"] = "";
                        }
                        else
                        {
                            // otherwise, show the group rate
                            row["DisplayRate"] = row["OrderGroupForeignRate"];
                        }

                        // Set the previous order group id for the next iteration
                        previousOrderGroupId = orderGroupId;
                    }
                    else
                    {
                        previousOrderGroupId = string.Empty;
                        row["DisplayRate"] = row["ForeignRate"];
                    }
                }
                else
                {
                    row["DisplayRate"] = row["ForeignRate"];

                    if (row["OrderGroupId"].ToString() != "")
                    {
                        previousOrderGroupId = row["OrderGroupId"].ToString();
                    }
                    else
                    {
                        previousOrderGroupId = string.Empty;
                    }
                }

                previousRow = row;
            }

            this.grdOrders.DataSource = orderData;

        }
    }
}
