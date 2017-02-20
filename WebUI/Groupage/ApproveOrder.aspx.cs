using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Transactions;

using Telerik.Web.UI;

using System.Collections.Generic;

namespace Orchestrator.WebUI.Groupage
{

    public partial class ApproveOrder : Orchestrator.Base.BasePage
    {
        #region Properties

        private CultureInfo _currentCulture = null;
        protected CultureInfo CurrentCulture
        {
            get { return _currentCulture == null ? new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture) : _currentCulture; }
            set { _currentCulture = value; }
        }

        private DateInputSetting _dipCollectionDate = null;
        private DateInputSetting CollectionDateManager
        {
            get
            {
                if (_dipCollectionDate == null)
                    _dipCollectionDate = rimApproveOrder.GetSettingByBehaviorID("DateInputBehavior1") as DateInputSetting;

                return _dipCollectionDate;
            }
        }

        private DateInputSetting _dipDeliveryDate = null;
        private DateInputSetting DeliveryDateManager
        {
            get
            {
                if (_dipDeliveryDate == null)
                    _dipDeliveryDate = rimApproveOrder.GetSettingByBehaviorID("DateInputBehavior2") as DateInputSetting;

                return _dipDeliveryDate;
            }
        }

        private DateInputSetting _dipTime = null;
        private DateInputSetting TimeManager
        {
            get
            {
                if (_dipTime == null)
                    _dipTime = rimApproveOrder.GetSettingByBehaviorID("TimeInputBehavior1") as DateInputSetting;

                return _dipTime;
            }
        }

        private NumericTextBoxSetting _dipNumeric = null;
        private NumericTextBoxSetting RateManager
        {
            get
            {
                if (_dipNumeric == null)
                {
                    _dipNumeric = rimApproveOrder.GetSettingByBehaviorID("NumericBehavior1") as NumericTextBoxSetting;
                    _dipNumeric.Culture = CurrentCulture;
                }

                return _dipNumeric;
            }
        }

        #endregion

        #region Templates

        private const string unApprovedPoint = @"<a href=""{0}"" target=""_blank"">{1}</a>";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.ApproveOrder);

            string args = Request.Params["__EVENTARGUMENT"];

            if (!this.IsPostBack || args.ToLower() == "refresh")
                this.ordersRadGrid.Rebind();

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.ordersRadGrid.NeedDataSource += new GridNeedDataSourceEventHandler(ordersRadGrid_NeedDataSource);
            this.ordersRadGrid.ItemDataBound += new GridItemEventHandler(ordersRadGrid_ItemDataBound);
            this.ordersRadGrid.UpdateCommand += new GridCommandEventHandler(ordersRadGrid_UpdateCommand);

            this.btnConfirmOrdersTop.Click += new EventHandler(btnConfirmOrders_Click);
            this.btnRejectOrdersTop.Click += new EventHandler(btnRejectOrders_Click);
            this.btnRefreshTop.Click += new EventHandler(btnRefresh_Click);
            this.btnSaveChanges.Click += new EventHandler(btnSaveChanges_Click);
            this.btnSaveChangesShortcut.Click += new EventHandler(btnSaveChanges_Click);
        }

        #region Private Methods

        private void BindData()
        {
            this.ordersRadGrid.DataSource = null;
            Facade.IOrder facOrder = new Facade.Order();

            int clientIdentityId = 0;
            try
            {
                clientIdentityId = Convert.ToInt32(Request.QueryString["iId"]);
            }
            catch
            {
                clientIdentityId = -1;
            }

            DataSet orderData = null;
            if (clientIdentityId > 0)
                orderData = facOrder.GetOrdersForClientAndStatus(clientIdentityId, eOrderStatus.Awaiting_Approval);
            else
                orderData = facOrder.GetOrders(eOrderStatus.Awaiting_Approval);

            this.ordersRadGrid.DataSource = orderData;
            this.btnConfirmOrdersTop.Visible = orderData.Tables[0].Rows.Count > 0;
            this.btnRejectOrdersTop.Visible = orderData.Tables[0].Rows.Count > 0;
        }

        private List<int> GetSelectedOrderIds()
        {
            List<int> orderIDs = new List<int>();

            if (orderIDs == null)
                orderIDs = new List<int>();

            orderIDs.Clear();
            int orderID = 0;
            foreach (GridItem row in ordersRadGrid.Items)
            {
                CheckBox chk = row.FindControl("chkSelectOrder") as CheckBox;
                HtmlInputHidden hidOrderId = row.FindControl("hidOrderId") as HtmlInputHidden;

                if (chk != null && chk.Checked)
                {
                    orderID = int.Parse(hidOrderId.Value);
                    orderIDs.Add(orderID);
                }
            }

            return orderIDs;
        }

        #endregion

        #region Events

        #region Grid

        private void ordersRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            this.BindData();
        }

        void ordersRadGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {

        }

        void ordersRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                GridItem item = e.Item as GridItem;
                DataRowView drv = e.Item.DataItem as DataRowView;

                ePointState collectionPointState = (ePointState)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["CollectionPointStateId"].ToString());
                ePointState deliveryPointState = (ePointState)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["deliveryPointStateId"].ToString());

                int collectionPointId = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["CollectionPointId"].ToString());
                int deliveryPointId = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["DeliveryPointId"].ToString());

                DateTime collectAt = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["CollectionDateTime"].ToString());
                DateTime collectAtBy = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["CollectionByDateTime"].ToString());
                DateTime deliverAt = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["DeliveryDateTime"].ToString());
                DateTime deliverAtFrom = Convert.ToDateTime(((System.Data.DataRowView)e.Item.DataItem)["DeliveryFromDateTime"].ToString());

                bool collectionIsAnyTime = Convert.ToBoolean(((System.Data.DataRowView)e.Item.DataItem)["CollectionIsAnyTime"].ToString());
                bool deliveryIsAnyTime = Convert.ToBoolean(((System.Data.DataRowView)e.Item.DataItem)["DeliveryIsAnyTime"].ToString());

                int lcid = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["LCID"].ToString());
                decimal rate = Convert.ToDecimal(((System.Data.DataRowView)e.Item.DataItem)["ForeignRate"].ToString());
                HiddenField hidOrderID = e.Item.FindControl("hidOrderChanged") as HiddenField;

                TextBox txtCollectAt = e.Item.FindControl("txtCollectAt") as TextBox;
                TextBox txtCollectionAtTime = e.Item.FindControl("txtCollectionAtTime") as TextBox;
                TextBox txtDeliverAt = e.Item.FindControl("txtDeliverAt") as TextBox;
                TextBox txtDeliverAtTime = e.Item.FindControl("txtDeliverAtTime") as TextBox;
                TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;

                txtCollectAt.Attributes.Add("hidOrderID", hidOrderID.ClientID);
                txtCollectionAtTime.Attributes.Add("hidOrderID", hidOrderID.ClientID);
                txtDeliverAt.Attributes.Add("hidOrderID", hidOrderID.ClientID);
                txtDeliverAtTime.Attributes.Add("hidOrderID", hidOrderID.ClientID);
                txtRate.Attributes.Add("hidOrderID", hidOrderID.ClientID);

                CollectionDateManager.TargetControls.Add(new TargetInput(txtCollectAt.UniqueID, true));
                DeliveryDateManager.TargetControls.Add(new TargetInput(txtDeliverAt.UniqueID, true));

                TimeManager.TargetControls.Add(new TargetInput(txtCollectionAtTime.UniqueID, true));
                TimeManager.TargetControls.Add(new TargetInput(txtDeliverAtTime.UniqueID, true));

                RateManager.TargetControls.Add(new TargetInput(txtRate.UniqueID, true));

                txtCollectAt.Text = collectAt.ToString();
                txtCollectionAtTime.Text = collectAt.ToString();
                txtDeliverAt.Text = deliverAt.ToString();
                txtDeliverAtTime.Text = deliverAt.ToString();

                if (collectionIsAnyTime)
                    txtCollectionAtTime.Text = "";
                else
                    txtCollectAt.Enabled = txtCollectionAtTime.Enabled = (collectAt == collectAtBy);

                if (deliveryIsAnyTime)
                    txtDeliverAtTime.Text = "";
                else
                    txtDeliverAt.Enabled = txtDeliverAtTime.Enabled = (deliverAt == deliverAtFrom);

                txtRate.Text = rate.ToString();

                int orderId = Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["OrderId"].ToString());
                string collectionPointDescription = ((System.Data.DataRowView)e.Item.DataItem)["CollectionPointDescription"].ToString();
                string deliveryPointDescription = ((System.Data.DataRowView)e.Item.DataItem)["DeliveryPointDescription"].ToString();
                bool itemHasPointsThatRequireApproval = false;

                // Set the hidden field order id.
                HtmlInputHidden hidOrderId = (HtmlInputHidden)item.FindControl("hidOrderId");
                hidOrderId.Value = orderId.ToString();

                // Only show approval hyperlink if point is unapproved.
                if (collectionPointState == ePointState.Unapproved)
                {
                    itemHasPointsThatRequireApproval = true;
                    Literal litCollectFrom = item.FindControl("litCollectFrom") as Literal;
                    litCollectFrom.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, ((System.Data.DataRowView)e.Item.DataItem)["CollectionPointDescription"].ToString(), string.Format("javascript:openDialogWithScrollbars('ApproveOrderPoint.aspx?PointId={0}&OrderId={1}&Ptype={2}',0,0);", collectionPointId, orderId, "C"))));
                }
                else
                {
                    Label lblCollectFromPoint = (Label)item.FindControl("lblCollectFromPoint");
                    lblCollectFromPoint.Text = collectionPointDescription;
                }

                if (deliveryPointState == ePointState.Unapproved)
                {
                    itemHasPointsThatRequireApproval = true;
                    Literal litDeliverTo = item.FindControl("litDeliverTo") as Literal;
                    litDeliverTo.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, ((System.Data.DataRowView)e.Item.DataItem)["DeliveryPointDescription"].ToString(), string.Format("javascript:openDialogWithScrollbars('ApproveOrderPoint.aspx?PointId={0}&OrderId={1}&Ptype={2}',0,0);", deliveryPointId, orderId, "D"))));
                }
                else
                {
                    Label lblDeliverToPoint = (Label)item.FindControl("lblDeliverToPoint");
                    lblDeliverToPoint.Text = deliveryPointDescription;
                }

                CheckBox selectionCheckbox = (CheckBox)item.FindControl("chkSelectOrder");
                if (itemHasPointsThatRequireApproval)
                {
                    selectionCheckbox.Enabled = false;
                }
                else
                {
                    selectionCheckbox.Attributes.Add("onclick", "javascript:ChangeList(event,this);");
                }
            }
        }

        #endregion

        #region Buttons

        private void btnConfirmOrders_Click(object sender, EventArgs e)
        {
            List<int> orderIDs = this.GetSelectedOrderIds();

            if (orderIDs.Count > 0)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    facOrder.Update(orderIDs, eOrderStatus.Approved, ((Entities.CustomPrincipal)Page.User).UserName);

                    Facade.IOrderApproval facOrderApproval = new Facade.Order();

                    // Insert Order approval rows
                    facOrderApproval.Create(orderIDs, eOrderStatus.Approved, "", DateTime.Now, this.Page.User.Identity.Name);

                    // Send email to client user 
                    try
                    {
                        facOrderApproval.SendClientOrderApprovalEmail(orderIDs, eOrderStatus.Approved, txtRejectionReasonTop.Text, this.Page.User.Identity.Name);
                    }
                    catch
                    { /* Swallow error: We dont care if the email approval/rejection email failed */ }

                    ts.Complete();
                }
            }

            this.ordersRadGrid.Rebind();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.ordersRadGrid.Rebind();
        }

        private void btnRejectOrders_Click(object sender, EventArgs e)
        {
            List<int> orderIDs = this.GetSelectedOrderIds();

            if (orderIDs.Count > 0)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    //Change status to rejected when the new type is added.
                    facOrder.Update(orderIDs, eOrderStatus.Rejected, ((Entities.CustomPrincipal)Page.User).UserName);

                    Facade.IOrderApproval facOrderApproval = new Facade.Order();

                    // Insert Order approval rows
                    facOrderApproval.Create(orderIDs, eOrderStatus.Rejected, txtRejectionReasonTop.Text, DateTime.Now, this.Page.User.Identity.Name);

                    // Send email to client user 
                    facOrderApproval.SendClientOrderApprovalEmail(orderIDs, eOrderStatus.Rejected, txtRejectionReasonTop.Text, this.Page.User.Identity.Name);

                    ts.Complete();
                }
            }

            this.ordersRadGrid.Rebind();
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();
            Entities.Order order = null;
            foreach (GridDataItem gdi in ordersRadGrid.Items)
            {
                order = null;

                int orderID = int.Parse(ordersRadGrid.MasterTableView.DataKeyValues[gdi.ItemIndex]["OrderID"].ToString());
                HiddenField hidOrderChaned = gdi.FindControl("hidOrderChanged") as HiddenField;

                if (hidOrderChaned.Value == "true")
                {
                    TextBox txtCollectAt = gdi.FindControl("txtCollectAt") as TextBox;
                    TextBox txtCollectionAtTime = gdi.FindControl("txtCollectionAtTime") as TextBox;
                    TextBox txtDeliverAt = gdi.FindControl("txtDeliverAt") as TextBox;
                    TextBox txtDeliverAtTime = gdi.FindControl("txtDeliverAtTime") as TextBox;
                    TextBox txtRate = gdi.FindControl("txtRate") as TextBox;

                    order = facOrder.GetForOrderID(orderID);

                    DateTime collectionDateTime = Convert.ToDateTime(txtCollectAt.Text);
                    DateTime deliveryDateTime = Convert.ToDateTime(txtDeliverAt.Text);

                    if(txtCollectAt.Enabled && txtCollectionAtTime.Enabled)
                        if (txtCollectionAtTime.Text.Length > 0)
                        {
                            //If Timed Collection
                            DateTime collectionTime = Convert.ToDateTime(txtCollectionAtTime.Text);
                            collectionDateTime = collectionDateTime.Date.Add(new TimeSpan(collectionTime.Hour,collectionTime.Minute, 0));
                            order.CollectionDateTime = order.CollectionByDateTime = collectionDateTime;
                        }
                        else
                        {
                            //If Anytime Collection
                            order.CollectionDateTime = collectionDateTime;
                            order.CollectionByDateTime = collectionDateTime.Date.Add(new TimeSpan(23, 59, 0));
                        }

                    if(txtDeliverAt.Enabled && txtDeliverAtTime.Enabled)
                        if (txtDeliverAtTime.Text.Length > 0)
                        {
                            //If Time Delivery
                            DateTime deliveryTime = Convert.ToDateTime(txtDeliverAtTime.Text);
                            deliveryDateTime = deliveryDateTime.Date.Add(new TimeSpan(deliveryTime.Hour,deliveryTime.Minute, 0));
                            order.DeliveryDateTime = order.DeliveryFromDateTime = deliveryDateTime;
                        }
                        else
                        {
                            //If Anytime Delivery
                            order.DeliveryFromDateTime = deliveryDateTime;
                            order.DeliveryDateTime = deliveryDateTime.Date.Add(new TimeSpan(23, 59, 0));
                        }

                    order.ForeignRate = Convert.ToDecimal(txtRate.Text);

                    // Get the fields that can be amended
                    //RadDateInput dteCollectAt = gdi.FindControl("dteCollectAt") as RadDateInput;
                    //RadDateInput dteCollectAtTime = gdi.FindControl("dteCollectAtTime") as RadDateInput;
                    //RadDateInput dteDeliverAt = gdi.FindControl("dteDeliverAt") as RadDateInput;
                    //RadDateInput dteDeliverAtTime = gdi.FindControl("dteDeliverAtTime") as RadDateInput;
                    //RadNumericTextBox rntRate = gdi.FindControl("rntRate") as RadNumericTextBox;

                    //// update the Order
                    //order = facOrder.GetForOrderID(orderID);

                    //DateTime collectionDateTime = dteCollectAt.SelectedDate.Value;
                    //if (dteCollectAtTime.SelectedDate.HasValue)
                    //{
                    //    collectionDateTime = collectionDateTime.Add(new TimeSpan(dteCollectAtTime.SelectedDate.Value.Hour, dteCollectAtTime.SelectedDate.Value.Minute, 0));
                    //    order.CollectionByDateTime = order.CollectionDateTime;
                    //}
                    //else
                    //{
                    //    order.CollectionDateTime = collectionDateTime;
                    //    order.CollectionByDateTime = collectionDateTime.Add(new TimeSpan(23, 59, 0));
                    //}

                    //DateTime deliveryDateTime = dteDeliverAt.SelectedDate.Value;
                    //if (dteDeliverAtTime.SelectedDate.HasValue)
                    //{
                    //    deliveryDateTime = deliveryDateTime.Add(new TimeSpan(dteDeliverAtTime.SelectedDate.Value.Hour, dteDeliverAtTime.SelectedDate.Value.Minute, 0));
                    //}
                    //else
                    //{
                    //    order.DeliveryFromDateTime = deliveryDateTime;
                    //    order.DeliveryDateTime = deliveryDateTime.Add(new TimeSpan(23, 59, 0));
                    //}

                    //order.ForeignRate = (decimal)rntRate.Value;

                    facOrder.Update(order, Page.User.Identity.Name);
                    order = null;
                }

            }

            ordersRadGrid.Rebind();

        }

        #endregion

        #endregion
    }
}
