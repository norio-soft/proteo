using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Telerik.Web.UI;
using System.Globalization;

namespace Orchestrator.WebUI.Invoicing.Groupage
{
    public partial class FlagForInvoicing : Orchestrator.Base.BasePage
    {
        #region Page Load/Init

        bool m_isFromDash = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!IsPostBack && !Page.IsCallback)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.NeedDataSource += new GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            grdOrders.ItemCreated += new GridItemEventHandler(grdOrders_ItemCreated);

            grdOrders.PreRender += new EventHandler(grdOrders_PreRender);

            this.btnEditRates.Click += new EventHandler(btnEditRates_Click);
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            btnSaveChanges.Click += new EventHandler(btnSaveChanges_Click);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);

        }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        #endregion

        private void ConfigureDisplay()
        {
            System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar();

            Facade.BusinessType facBusinessType = new Facade.BusinessType();
            Facade.IOrderServiceLevel facOrderServiceLevel = new Facade.Order();

            cblBusinessType.DataSource = facBusinessType.GetAll();
            cblBusinessType.DataBind();

            foreach (ListItem li in cblBusinessType.Items)
                li.Selected = true;

            cblServiceLevel.DataSource = facOrderServiceLevel.GetAll();
            cblServiceLevel.DataBind();

            foreach (ListItem li in cblServiceLevel.Items)
                li.Selected = true;

            string referencesSortExpression = Globals.Configuration.FlagForInvoicingReferencesSortExpression;
            if (!string.IsNullOrEmpty(referencesSortExpression))
                grdOrders.Columns.FindByUniqueName("References").SortExpression = referencesSortExpression;

            int IdentityID = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["identityid"]))
                int.TryParse(Request.QueryString["identityid"].ToString(), out IdentityID);

            string MinDate = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["MinDate"]))
                MinDate = Request.QueryString["MinDate"];

            if (IdentityID > 0 && MinDate.Length > 0)
            {
                m_isFromDash = true;
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(IdentityID);

                cboClient.SelectedValue = IdentityID.ToString();
                cboClient.Text = name;

                DateTime startDate = new DateTime();
                DateTime.TryParse(MinDate, out startDate);

                rdiStartDate.SelectedDate = startDate;
                rdiEndDate.SelectedDate = System.DateTime.Today;

                if (startDate >= System.DateTime.Today)
                    rdiEndDate.SelectedDate = startDate.AddDays(1);

                rdoListInvoiceType.Items[0].Selected = false;
                rdoListInvoiceType.Items[2].Selected = true;

                grdOrders.Rebind();
            }
           
        }

        #region Button Events

        private void btnEditRates_Click(object sender, EventArgs e)
        {
            if (this.grdOrders.Items.Count > 0)
            {
                Button editRates = (Button)sender;
                if (editRates.Text == "Edit Rates")
                {
                    foreach (GridItem item in grdOrders.MasterTableView.Items)
                    {
                        if (item is GridEditableItem)
                        {
                            GridEditableItem editableItem = item as GridDataItem;
                            editableItem.Edit = true;
                        }
                    }
                    this.btnEditRates.Text = "Update Rates";
                }
                else
                {
                    foreach (GridEditableItem editedItem in grdOrders.EditItems)
                    {
                        bool orderInGroupOverridden = false;
                        bool updateOrder = false;
                        // UpdateRate if it has changed

                        HtmlInputText txtRate = editedItem.FindControl("txtOrderRate") as HtmlInputText;
                        int orderId = int.Parse(editedItem.GetDataKeyValue("OrderID").ToString());

                        Facade.IOrder facOrder = new Facade.Order();
                        // get the data item so we can compare the rate before it was changed
                        Entities.Order order = facOrder.GetForOrderID(orderId);

                        CheckRateInformation(order, ref orderInGroupOverridden, ref updateOrder);

                        CultureInfo culture = new CultureInfo(order.LCID);
                        decimal newRate = Decimal.Parse(txtRate.Value, NumberStyles.Any, culture);

                        if (newRate != order.ForeignRate)
                        {
                            // Mark as overridden... update the rate on the order.
                            order.ForeignRate = newRate;
                            if (!order.IsTariffOverride)
                            {
                                order.IsTariffOverride = true;
                                order.TariffOverrideDate = DateTime.Now;
                                order.TariffOverrideUserID = User.Identity.Name;
                            }

                            // Recalculate the GBP amounts
                            BusinessLogicLayer.IExchangeRates blER = new BusinessLogicLayer.ExchangeRates();
                            BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(order.LCID, order.CollectionDateTime);
                            order.Rate = currencyConverter.ConvertToLocal(order.ForeignRate);

                            updateOrder = true;
                        }

                        // Update the order as the CheckRateInformation method may have amended the order
                        if (updateOrder)
                            facOrder.Update(order, User.Identity.Name);

                        if (orderInGroupOverridden && order.OrderGroupID > 0)
                        {
                            Facade.IOrderGroup facOrderGroup = new Facade.Order();
                            Entities.OrderGroup orderGroup = facOrderGroup.Get(order.OrderGroupID);
                            orderGroup.TariffRateDescription = "Overridden";
                            facOrderGroup.Update(orderGroup, this.Page.User.Identity.Name);
                        }

                        editedItem.Edit = false;
                    }
                    this.btnEditRates.Text = "Edit Rates";


                }

                this.grdOrders.Rebind();
            }
        }

        private void CheckRateInformation(Entities.Order order, ref bool orderInGroupOverridden, ref bool updateOrder)
        {
            Facade.IOrder facOrder = new Facade.Order();
            IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;
            Repositories.DTOs.RateInformation correctRateInformation = null;

            try
            {
                Entities.VigoOrder vigoOrder = null;
                Facade.IVigoOrder facVigoOrder = new Facade.Order();
                vigoOrder = facVigoOrder.GetForOrderId(order.OrderID);

                if (vigoOrder != null)
                    correctRateInformation = facOrder.GetRate(order, false, false, out surcharges, vigoOrder);
                else
                    correctRateInformation = facOrder.GetRate(order, false, false, out surcharges);
            }
            catch (ApplicationException aex)
            {
                if (!aex.Message.StartsWith("Postcode"))
                    throw;
            }

            if (correctRateInformation != null)
            {
                // A rate was detected
                if (order.IsAutorated == false)
                {
                    updateOrder = true;

                    if (order.ForeignRate == correctRateInformation.ForeignRate)
                    {
                        order.IsAutorated = true;
                    }
                    else
                    {
                        orderInGroupOverridden = true;
                        order.IsAutorated = true;
                        order.TariffOverrideDate = DateTime.Now;
                        order.TariffOverrideUserID = this.Page.User.Identity.Name;
                        order.IsTariffOverride = true;
                    }
                }
            }
        }

        void btnSaveChanges_Click(object sender, EventArgs e)
        {
            List<int> flaggedOrderIDs = new List<int>();

            foreach (GridItem item in grdOrders.Items)
            {
                if (item is GridDataItem)
                {
                    using (HtmlInputCheckBox chk = (HtmlInputCheckBox)item.FindControl("chkOrder"))
                    {
                        if (chk.Checked)
                        {
                            int orderID =
                            int.Parse(item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString());
                            flaggedOrderIDs.Add(orderID);
                        }
                    }
                    
                }
            }

            if (flaggedOrderIDs.Count > 0)
            {
                // Update the orders that have been flagged.
                Facade.IOrder facOrder = new Facade.Order();

                if (rdoListOrderTypes.SelectedItem.Value == "Unflagged")
                    facOrder.FlagAsReadyToInvoice(flaggedOrderIDs, ((Entities.CustomPrincipal)Page.User).UserName);
                else
                    flaggedOrderIDs.ForEach(fo => facOrder.UnflagForInvoicing(fo));

                grdOrders.Rebind();
            }
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            grdOrders.Rebind();

            btnSaveChanges.Text = (rdoListOrderTypes.SelectedItem.Value == "Unflagged") ? "Flag Selected Orders" : "Unflag Selected Orders";
        }

        #endregion

        #region Combo Events

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text, true);
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

        #region Grid Events

        void grdOrders_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
                if (e.Item.DataItem != null)
                {
                    DataRowView orderRow = (DataRowView)e.Item.DataItem;
                    GridEditableItem item = e.Item as GridEditableItem;
                    GridTemplateColumnEditor editor = (GridTemplateColumnEditor)item.EditManager.GetColumnEditor("Rate");
                    HtmlInputText txtRate = (HtmlInputText)editor.ContainerControl.FindControl("txtOrderRate");

                    CultureInfo culture = new CultureInfo(orderRow["LCID"] == DBNull.Value ? 2057: (int)orderRow["LCID"]);
                    txtRate.Value = Convert.ToDecimal(orderRow["ForeignRate"].ToString()).ToString("C", culture);
                    txtRate.Attributes.Add("OldRate", txtRate.Value);
                }
        }

        void grdOrders_PreRender(object sender, System.EventArgs e)
        {
            foreach (GridColumn column in grdOrders.Columns)
            {
                if (column.UniqueName == "DocketNo")
                    (column as GridBoundColumn).HeaderText = Globals.Configuration.SystemDocketNumberText;

                if (column.UniqueName == "LoadNo")
                    (column as GridBoundColumn).HeaderText = Globals.Configuration.SystemLoadNumberText;
            }
            grdOrders.Rebind();
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {

                DataRowView drv = (DataRowView) e.Item.DataItem;
                using (Repeater rep = e.Item.FindControl("repReferences") as Repeater)
                {
                    int orderID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"].ToString());
                    DataRow[] references = ((DataSet) grdOrders.DataSource).Tables[1].Select("OrderID = " + orderID);
                    List<DataRow> listReferences = new List<DataRow>(references);

                //    //// Add the customer order number and delivery order number!
                //    //DataRow deliveryOrderNumber = ((DataSet) grdOrders.DataSource).Tables[1].NewRow();
                //    //deliveryOrderNumber["OrderID"] = orderID;
                //    //deliveryOrderNumber["Description"] = "Delivery Order Number";
                //    //deliveryOrderNumber["Reference"] = ((DataRowView) e.Item.DataItem)["DeliveryOrderNumber"].ToString();
                //    //listReferences.Insert(0, deliveryOrderNumber);

                //    //DataRow customerOrderNumber = ((DataSet) grdOrders.DataSource).Tables[1].NewRow();
                //    //customerOrderNumber["OrderID"] = orderID;
                //    //customerOrderNumber["Description"] = "Customer Order Number";
                //    //customerOrderNumber["Reference"] = ((DataRowView) e.Item.DataItem)["CustomerOrderNumber"].ToString();
                //    //listReferences.Insert(0, customerOrderNumber);

                    if (rep != null)
                    {
                        rep.DataSource = listReferences;
                        rep.DataBind();
                    }
                }

                // Problem notification.
                if ((bool)drv["HasProblem"])
                {
                    e.Item.BackColor = System.Drawing.ColorTranslator.FromHtml("#f2dfdf");
                }

                if ((bool)drv["OnHold"] || (bool)drv["IsExcludedFromInvoicing"])
                {
                    e.Item.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffc2b2");
                }

                // Selection checkbox.
                using (var chk = (HtmlInputCheckBox)e.Item.FindControl("chkOrder"))
                {
                    chk.Attributes.Add("Index", e.Item.ItemIndex.ToString());
                    chk.Attributes.Add("OrderGroupID",drv["OrderGroupID"].ToString());

                    var tooltip = string.Empty;
                    var enabled = true;

                    if ((bool)drv["OnHold"])
                    {
                        string onHoldReason = drv["OnHoldReason"] == DBNull.Value ? null : (string)drv["OnHoldReason"];

                        if (String.IsNullOrWhiteSpace(onHoldReason))
                            tooltip = "Client On Hold - No Reason Given";
                        else
                            tooltip = "Client On Hold - Reason: " + onHoldReason;

                        enabled = false;
                    }

                    if((bool)drv["IsExcludedFromInvoicing"])
                    {
                        if ((bool)drv["OnHold"])
                            tooltip = tooltip + ". Also, the Customer is set to be Excluded From Invoicing.";
                        else
                        {
                            tooltip = "The Customer is set to be Excluded From Invoicing.";
                            enabled = false;
                        }
                    }
                    //Enable or disable the row
                    e.Item.Enabled = enabled;
                    //chk.Disabled = !enabled;
                    chk.Attributes["title"] = tooltip;
                }

                using (Label lblCharge = e.Item.FindControl("lblCharge") as Label)
                {
                    if (lblCharge != null)
                    {
                        CultureInfo culture = new CultureInfo(drv["LCID"] == DBNull.Value ? 2057 : (int)drv["LCID"]);
                        decimal foreignRate = Convert.ToDecimal(drv["ForeignRate"]);

                        // this will display both rates if the foreign rate and normal rate are different.
                        if (drv["ForeignRate"].ToString() != drv["Rate"].ToString())
                        {
                            lblCharge.Text = foreignRate.ToString().Length < 2 ? "&nbsp;" : foreignRate.ToString("C", culture);
                        }
                    }
                }

                using (HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces"))
                {
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
                }

                HtmlAnchor hypOrder = (HtmlAnchor)e.Item.FindControl("hypOrder");
                //var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;
                string openOrderJS = dlgOrder.GetOpenDialogScript("wiz=true&oID=" + drv["OrderID"].ToString());
                hypOrder.HRef = "javascript:" + openOrderJS;

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
        }

        void grdOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (!IsCallback && (IsPostBack || m_isFromDash))
            {
                int clientID;
                bool getUnflaggedOrders;
                StringBuilder bts = new StringBuilder();
                StringBuilder sls = new StringBuilder();

                int.TryParse(cboClient.SelectedValue, out clientID);
                getUnflaggedOrders = rdoListOrderTypes.SelectedItem.Value == "Unflagged";

                foreach (ListItem li in cblBusinessType.Items)
                    if (li.Selected)
                        bts.Append(bts.Length > 0 ? "," + li.Value : li.Value);

                foreach (ListItem li in cblServiceLevel.Items)
                    if (li.Selected)
                        sls.Append(sls.Length > 0 ? "," + li.Value : li.Value);



                Facade.IOrder facOrder = new Facade.Order();
                DataSet ds = facOrder.GetOrdersThatCanBeFlaggedForInvoicing(
                                                                            rdiStartDate.SelectedDate.Value,
                                                                            rdiEndDate.SelectedDate.Value,
                                                                            rdoListInvoiceType.Items[0].Selected || rdoListInvoiceType.Items[2].Selected,
                                                                            rdoListInvoiceType.Items[1].Selected || rdoListInvoiceType.Items[2].Selected,
                                                                            rblJobs.Items[0].Selected || rblJobs.Items[1].Selected,
                                                                            rblJobs.Items[0].Selected || rblJobs.Items[2].Selected,
                                                                            rblPricing.Items[0].Selected || rblPricing.Items[1].Selected,
                                                                            rblPricing.Items[0].Selected || rblPricing.Items[2].Selected,
                                                                            rblPricing.Items[0].Selected || rblPricing.Items[3].Selected,
                                                                            bts.ToString(),
                                                                            sls.ToString(),
                                                                            getUnflaggedOrders,
                                                                            clientID,
                                                                            chkShowExcluded.Checked);

                grdOrders.DataSource = ds;
            }
            else
                // No need to show on first load.
                grdOrders.DataSource = null;
        }

        #endregion
    }
}