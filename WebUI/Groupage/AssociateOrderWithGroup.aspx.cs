using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Orchestrator.Entities;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.Linq;

namespace Orchestrator.WebUI.Groupage
{
    public partial class AssociateOrderWithGroup : Orchestrator.Base.BasePage
    {
        #region Constants

        private const string _orderGroupID_QS = "ogid";

        #endregion

        #region Fields

        private int _orderGroupID = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves a list of the order ids selected by the user for association.
        /// </summary>
        private List<int> Orders
        {
            get
            {
                List<int> orders = new List<int>();
                foreach (GridItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked && chk.Enabled)
                    {
                        int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                        if (!orders.Contains(orderID))
                            orders.Add(orderID);
                    }
                }

                return orders;
            }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString[_orderGroupID_QS]))
                int.TryParse(Request.QueryString[_orderGroupID_QS], out _orderGroupID);

            if (!IsPostBack)
            {
                ConfigureDisplay();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cboResource.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
            cboSubContractor.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            btnSearch.Click += new EventHandler(btnSearch_Click);

            grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            grdOrders.SortCommand += new Telerik.Web.UI.GridSortCommandEventHandler(grdOrders_SortCommand);

            btnAssociate.Click += new EventHandler(btnAssociate_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
        }

        #endregion

        /// <summary>
        /// Prepares the page for viewing.
        /// </summary>
        private void ConfigureDisplay()
        {
            // Populate master page information
            //this.Master.WizardTitle = "Find Orders to Associate";

            // Bind the order states
            List<string> orderStateNames = new List<string>(Enum.GetNames(typeof(eOrderStatus)));
            orderStateNames.Remove(eOrderStatus.Invoiced.ToString());
            orderStateNames.Remove(eOrderStatus.Cancelled.ToString());
            cblOrderStatus.DataSource = orderStateNames;
            cblOrderStatus.DataBind();

            // Mark approved as being the only item checked.
            cblOrderStatus.Items.FindByValue(eOrderStatus.Approved.ToString()).Selected = true;

            // Default date range.
            dteStartDate.SelectedDate = DateTime.Today;
            dteEndDate.SelectedDate = DateTime.Today.AddDays(2);

            // Set the client to match that of the orders in the group.
            Facade.IOrderGroup facOrderGroup = new Facade.Order();
            Entities.OrderGroup orderGroup = facOrderGroup.Get(_orderGroupID);
            if (orderGroup != null && orderGroup.Orders.Count > 0)
            {
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                int orderGroupCustomerIdentityID = orderGroup.Orders[0].CustomerIdentityID;
                cboClient.Enabled = false;
                cboClient.Text = facOrganisation.GetNameForIdentityId(orderGroupCustomerIdentityID);
                cboClient.SelectedValue = orderGroupCustomerIdentityID.ToString();
            }
        }

        /// <summary>
        /// Display the infringments returned from the facade call.
        /// </summary>
        /// <param name="result">The result returned from the facade call containing the infringements.</param>
        private void DisplayInfringments(FacadeResult result)
        {
            // The update failed, display infringements.
            ruleInfringements.Infringements = result.Infringements;
            ruleInfringements.DisplayInfringments();
            ruleInfringements.Visible = true;
        }

        /// <summary>
        /// Displays the message supplied.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void DisplayMessage(string message)
        {
            lblNote.Text = message;
            pnlConfirmation.Visible = true;
        }

        /// <summary>
        /// Formats the datetime to take into account the presence of isAnytime flag.
        /// </summary>
        /// <param name="date">The date format.</param>
        /// <param name="isAnytime">The anytime flag.</param>
        /// <returns>The date in a format suitable for display.</returns>
        protected static string GetDate(DateTime date, bool isAnytime)
        {
            string retVal;

            if (isAnytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        #region Event Handlers

        #region Button Event Handlers

        void btnAssociate_Click(object sender, EventArgs e)
        {
            List<int> orderIDs = Orders;
            if (orderIDs.Count > 0)
            {
                this.RemoveOrdersFromOriginalGroup();

                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                FacadeResult result =
                    facOrderGroup.AddOrdersToGroup(_orderGroupID, orderIDs, ((CustomPrincipal)Page.User).UserName);
                if (result.Success)
                {
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "__dialogCallBack(window, 'refresh');", true);
                    string orderIdsCSV = String.Empty;

                    foreach (int orderId in orderIDs)
                        if (String.IsNullOrEmpty(orderIdsCSV))
                            orderIdsCSV += orderId.ToString();
                        else
                            orderIdsCSV += "," + orderId.ToString();

                    this.ReturnValue = orderIdsCSV;
                    grdOrders.Rebind();
                }
                else
                {
                    // Cause the infringements to be displayed.
                    DisplayInfringments(result);
                }
            }
        }

        private void RemoveOrdersFromOriginalGroup()
        {
            IDictionary<int, List<int>> orderGroupIds = new Dictionary<int, List<int>>();
            Facade.IOrder facOrder = new Facade.Order();
            Facade.IOrderGroup facOrderGroup = new Facade.Order();

            if (Orders.Count > 0)
                foreach (int OrderId in Orders)
                { 
                    Entities.Order order = facOrder.GetForOrderID(OrderId);
                    KeyValuePair<int,List<int>> item = orderGroupIds.FirstOrDefault(kv => kv.Key == order.OrderGroupID);

                    if (item.Value != null && item.Value.Count > 0)
                        item.Value.Add(OrderId);
                    else
                        orderGroupIds.Add(order.OrderGroupID, new List<int>() { OrderId });
                }

            foreach (KeyValuePair<int,List<int>> pair in orderGroupIds)
                facOrderGroup.RemoveOrdersFromGroup(pair.Key, pair.Value, ((CustomPrincipal)Page.User).UserName);
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            grdOrders.Rebind();
        }

        #endregion

        #region Grid Event Handlers

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView) e.Item.DataItem;
                CheckBox chk = e.Item.FindControl("chkOrderID") as CheckBox;

                if (chk != null)
                {
                    int orderGroupID;
                    int.TryParse(drv["OrderGroupID"].ToString(), out orderGroupID);
                    chk.Enabled = orderGroupID != _orderGroupID;
                    chk.Attributes.Add("orderGroupId", orderGroupID.ToString());

                    chk.Attributes.Add("onclick",
                   "javascript:ChangeList(event, this," + ((DataRowView)e.Item.DataItem)["OrderID"] +
                   "," + ((DataRowView)e.Item.DataItem)["NoPallets"] + "," + orderGroupID + ");");
                }

                HtmlAnchor hypRun = e.Item.FindControl("hypRun") as HtmlAnchor;
                if(hypRun != null)
                    if(drv["JobId"] != DBNull.Value)
                    {
                        hypRun.HRef = "javascript:ViewJob(" + drv["JobId"].ToString() + ");";
                        hypRun.InnerHtml = drv["JobId"].ToString();
                    }
            }
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack && string.IsNullOrEmpty(Request.QueryString["rcbID"]))
            {
                // Search for orders based on Date Range Status and text
                // Determine the parameters
                List<int> orderStatusIDs = new List<int>();
                foreach (ListItem li in cblOrderStatus.Items)
                {
                    if (li.Selected)
                    {
                        orderStatusIDs.Add((int) ((eOrderStatus) Enum.Parse(typeof (eOrderStatus), li.Value)));
                    }
                }

                // Retrieve the client id, resource id, and sub-contractor identity id.
                int clientID;
                int.TryParse(cboClient.SelectedValue, out clientID);
                int resourceID;
                int.TryParse(cboResource.SelectedValue, out resourceID);
                int subContractorIdentityID;
                int.TryParse(cboSubContractor.SelectedValue, out subContractorIdentityID);

                // Find the orders.
                Facade.IOrder facOrder = new Facade.Order();
                DataSet orderData;
                try
                {
                    List<int> businessTypes = new List<int>();
                    
                    Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                    DataSet dsBusinessTypes = facBusinessType.GetAll();

                    foreach(DataRow row in dsBusinessTypes.Tables[0].Rows)
                        businessTypes.Add(Convert.ToInt32(row["BusinessTypeID"]));

                    orderData =
                        facOrder.Search(orderStatusIDs, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value, txtSearch.Text,
                                        cboSearchAgainstDate.Items[0].Selected || cboSearchAgainstDate.Items[2].Selected,
                                        cboSearchAgainstDate.Items[1].Selected || cboSearchAgainstDate.Items[2].Selected,
                                        false, false, clientID, resourceID, subContractorIdentityID, businessTypes, 0, 0, 0);
                }
                catch (SqlException exc)
                {
                    if (exc.Message.StartsWith("Timeout expired."))
                    {
                        // A timeout exception has been encountered, instead of throwing the error page, instruct the user to refine their search.
                        DisplayMessage(
                            "Your query is not precise enough, please provide additional information or narrow the date/order state range.");

                        // Communicate the details of the exception to support.
                        string methodCall = "Facade.IOrder.Search('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}); encountered by {9}";
                        Utilities.SendSupportEmailHelper(
                            string.Format(methodCall,
                                          Entities.Utilities.GetCSV(orderStatusIDs),
                                          dteStartDate.SelectedDate.Value,
                                          dteEndDate.SelectedDate.Value,
                                          txtSearch.Text,
                                          cboSearchAgainstDate.Items[0].Selected ||
                                          cboSearchAgainstDate.Items[2].Selected,
                                          cboSearchAgainstDate.Items[1].Selected ||
                                          cboSearchAgainstDate.Items[2].Selected,
                                          clientID,
                                          resourceID,
                                          subContractorIdentityID,
                                          ((Entities.CustomPrincipal) Page.User).UserName), exc);

                        orderData = null;
                    }
                    else
                        throw;
                }

                grdOrders.DataSource = orderData;
            }
        }

        static void grdOrders_SortCommand(object source, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            if (!e.Item.OwnerTableView.SortExpressions.ContainsExpression(e.SortExpression))
            {
                GridSortExpression gridSortExpression = new GridSortExpression();
                gridSortExpression.FieldName = e.SortExpression;
                gridSortExpression.SortOrder = GridSortOrder.Ascending;

                e.Item.OwnerTableView.SortExpressions.AddSortExpression(gridSortExpression);
            }
        }

        #endregion

        #region Combobox Event Handlers

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset, dt.Rows.Count);
            }
        }

        void cboResource_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboResource.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, false, true);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboResource.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset, dt.Rows.Count);
            }
        }

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text, false);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset, dt.Rows.Count);
            }
        }

        #endregion

        #endregion
    }
}