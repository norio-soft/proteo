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
    public partial class InvoiceReview : Orchestrator.Base.BasePage
    {
        #region Control Fields

        private const string _VS_WorkflowInstanceID = "_VS_WorkflowInstanceID";
        private const string _VS_StayOpen = "_VS_StayOpen";

        #endregion

        #region Properties

        /// <summary>
        /// Access the workflow instance id being used during the approval process.
        /// </summary>
        private Guid WorkflowInstanceID
        {
            get
            {
                if (ViewState[_VS_WorkflowInstanceID] == null)
                    return Guid.Empty;
                else
                    return (Guid)ViewState[_VS_WorkflowInstanceID];
            }
            set { ViewState[_VS_WorkflowInstanceID] = value; }
        }

        private int StayOpen
        {
            get
            {
                if (ViewState[_VS_StayOpen] == null)
                    return -1;
                else
                    return (int)ViewState[_VS_StayOpen];
            }
            set { ViewState[_VS_StayOpen] = value; }
        }

        private DataSet dsTaxRates
        {
            get
            {
                DataSet dsTaxRates = null;
                if (ViewState["dsTaxRates"] != null)
                {
                    dsTaxRates = (DataSet)ViewState["dsTaxRates"];
                }

                return dsTaxRates;
            }
            set
            {
                ViewState["dsTaxRates"] = value;
            }
        }

        private string PreInvoiceTableName
        {
            get { return gvPreInvoices.MasterTableView.Name; }
        }

        private string PreInvoiceItemTableName
        {
            get { return gvPreInvoices.MasterTableView.DetailTables[0].Name; }
        }

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!IsPostBack && !Page.IsCallback)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["wid"]))
                {
                    try
                    {
                        WorkflowInstanceID = new Guid(Request.QueryString["wid"]);
                    }
                    catch { }
                }

                if (WorkflowInstanceID == Guid.Empty)
                    Response.Redirect("ReviewableGroups.aspx");

                btnApply.OnClientClick = "this.style.display = 'none';";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvPreInvoices.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvPreInvoices_NeedDataSource);
            gvPreInvoices.ItemCommand += new GridCommandEventHandler(gvPreInvoices_ItemCommand);
            gvPreInvoices.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(gvPreInvoices_ItemDataBound);
            gvPreInvoices.DetailTableDataBind += new GridDetailTableDataBindEventHandler(gvPreInvoices_DetailTableDataBind);
            gvPreInvoices.PreRender += new EventHandler(gvPreInvoices_PreRender);
            btnApply.Click += new EventHandler(btnApply_Click);
        }

        void gvPreInvoices_PreRender(object sender, EventArgs e)
        {
            //foreach (GridDataItem gdi in this.gvPreInvoices.Items)
            //{
            //gdi.Expanded = true;
            //this.gvPreInvoices.Items[0].Expanded = true;
            if (StayOpen >= 0)
            {
                gvPreInvoices.Items[StayOpen].Expanded = true;
                StayOpen = -1;
            }
            //}
        }

        /// <summary>
        /// Use the hidden field view state persister for this page as the user could be visiting
        /// many different pages prior to regenerating invoices for example.
        /// </summary>
        protected override PageStatePersister PageStatePersister
        {
            get { return new HiddenFieldPageStatePersister(this); }
        }

        #endregion

        #region Private Methods

        private void AddReaction(RadioButton radioButton, string controlID)
        {
            radioButton.Attributes.Remove("onClick");
            radioButton.Attributes.Add("onClick", "javascript:SelectAllItems('" + controlID + "');");
        }

        #endregion

        #region Event Handlers

        #region Button Events

        void btnApply_Click(object sender, EventArgs e)
        {
            List<int> reject = new List<int>();
            List<int> regenerate = new List<int>();
            List<int> approve = new List<int>();
            List<int> approveAndPost = new List<int>();

            // React to the user's options.
            foreach (GridDataItem row in gvPreInvoices.Items)
            {
                if (row.OwnerTableView.Name == PreInvoiceTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                {
                    HtmlInputRadioButton rdoReject = row.FindControl("rdoReject") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoRegenerate = row.FindControl("rdoRegenerate") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApprove = row.FindControl("rdoApprove") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApproveAndPost = row.FindControl("rdoApproveAndPost") as HtmlInputRadioButton;
                    HtmlInputCheckBox chkUseHeadedPaper = row.FindControl("chkUseHeadedPaper") as HtmlInputCheckBox;

                    if (rdoReject.Checked)
                        reject.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                    else if (rdoRegenerate.Checked)
                        regenerate.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                    else if (rdoApprove.Checked)
                        approve.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                    else if (rdoApproveAndPost.Checked)
                        approveAndPost.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));

                    if (rdoApprove.Checked || rdoApproveAndPost.Checked)
                    {
                        if (chkUseHeadedPaper.Checked)
                        {
                            SetUseHeadedPaperParameter(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()), chkUseHeadedPaper.Checked);
                        }
                    }
                }
            }

            // Notify no-one.
            List<Contracts.DataContracts.NotificationParty> notificationParties = new List<Contracts.DataContracts.NotificationParty>();
            Entities.CustomPrincipal cp = (Entities.CustomPrincipal)Page.User;


            try
            {
                // Kick off the workflow (if there is anything to do).
                if (approveAndPost.Count + approve.Count + regenerate.Count + reject.Count > 0)
                {
                    GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                    gic.VerifyInvoices(approveAndPost.ToArray(), approve.ToArray(), regenerate.ToArray(), reject.ToArray(), notificationParties.ToArray(), cp.UserName);

                    Server.Transfer("reviewablegroups.aspx");
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException exc)
            {
                // Not possible to send message to workflow host - send email to support.
                Utilities.SendSupportEmailHelper("GenerateInvoiceClient.VerifyInvoices(int[], int[], int[], int[], Orchestrator.Entities.NotificationParty[], string)", exc);
                // Redirect user to an appropriate page.
                Server.Transfer("../../OfflineProcessInitiationFailed.aspx");
            }
        }

        #endregion

        #region Drop Down Events

        protected void cboTaxRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList cboTaxRate = (DropDownList)sender;
            int taxRateID = 0;
            if (int.TryParse(cboTaxRate.SelectedValue, out taxRateID))
            {
                GridDataItem gdi = (GridDataItem)cboTaxRate.Parent.Parent;
                int preInvoiceID = 0;
                if (int.TryParse(gdi.GetDataKeyValue("PreInvoiceID").ToString(), out preInvoiceID))
                {
                    Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                    facPreInvoice.UpdateTaxRate(preInvoiceID, taxRateID, ((Entities.CustomPrincipal)Page.User).UserName);
                    gvPreInvoices.MasterTableView.Rebind();
                }
            }
        }

        #endregion

        #region Grid Events

        void gvPreInvoices_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem parentItem = e.DetailTableView.ParentItem as GridDataItem;
            if (parentItem.Edit)
                return;

            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            e.DetailTableView.DataSource = facPreInvoice.GetItemsForPreInvoice(int.Parse(parentItem.GetDataKeyValue("PreInvoiceID").ToString()));
        }

        void gvPreInvoices_ItemCommand(object source, GridCommandEventArgs e)
        {
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            switch (e.CommandName.ToLower())
            {
                #region Client Reference Editing

                case "cancelclientreference":
                    e.Item.Edit = false;
                    gvPreInvoices.Rebind();
                    break;
                case "editclientreference":
                    e.Item.Edit = true;
                    gvPreInvoices.Rebind();
                    break;
                case "editpurchaseorderreference":
                    e.Item.Edit = true;
                    gvPreInvoices.Rebind();
                    break;
                case "updateclientreference":
                    e.Item.Edit = false;
                    string clientReference = string.Empty;

                    LinkButton lnkChangeClientReference = e.Item.FindControl("lnkChangeClientReference") as LinkButton;
                    TextBox txtClientReference = e.Item.FindControl("txtClientReference") as TextBox;

                    if (lnkChangeClientReference != null && txtClientReference != null)
                    {
                        string oldClientReference = lnkChangeClientReference.Text;
                        clientReference = txtClientReference.Text;

                        // Only apply the change if the client reference has been altered
                        if (oldClientReference != clientReference)
                        {
                            // Update the client reference
                            int preInvoiceID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("PreInvoiceID").ToString());
                            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                            facPreInvoice.UpdateClientInvoiceReference(preInvoiceID, clientReference, userName);
                        }
                    }
                    gvPreInvoices.Rebind();
                    break;
                case "updatepurchaseorderreference":
                    e.Item.Edit = false;
                    string purchaseOrderReference = string.Empty;

                    LinkButton lnkChangePurchaseOrderReference = e.Item.FindControl("lnkChangePurchaseOrderReference") as LinkButton;
                    TextBox txtPurchaseOrderReference = e.Item.FindControl("txtPurchaseOrderReference") as TextBox;

                    if (lnkChangePurchaseOrderReference != null && txtPurchaseOrderReference != null)
                    {
                        string oldPurchaseOrderReference = lnkChangePurchaseOrderReference.Text;
                        purchaseOrderReference = txtPurchaseOrderReference.Text;

                        // Only apply the change if the client reference has been altered
                        if (oldPurchaseOrderReference != purchaseOrderReference)
                        {
                            // Update the client reference
                            int preInvoiceID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("PreInvoiceID").ToString());
                            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                            facPreInvoice.UpdatePurchaseOrderReference(preInvoiceID, purchaseOrderReference, userName);
                        }
                    }
                    gvPreInvoices.Rebind();
                    break;

                #endregion

                #region Rate Editing

                //case "cancelrate":
                //    e.Item.Edit = false;
                //    e.Item.OwnerTableView.Rebind();
                //    gvPreInvoices.MasterTableView.Rebind();
                //    StayOpen = e.Item.OwnerTableView.ParentItem.ItemIndex;
                //    break;
                //case "editrate":
                //    e.Item.Edit = true;
                //    e.Item.OwnerTableView.Rebind();
                //    gvPreInvoices.MasterTableView.Rebind();
                //    StayOpen = e.Item.OwnerTableView.ParentItem.ItemIndex;
                //    break;
                case "update": // update rate and pallet spaces.
                    e.Item.Edit = false;
                    decimal rate = 0;

                    //LinkButton lnkChangeRate = e.Item.FindControl("lnkChangeRate") as LinkButton;
                    HiddenField hidOldRate = e.Item.FindControl("hidOldRate") as HiddenField;
                    TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;
                    GridDataItem parentItem = e.Item.OwnerTableView.ParentItem;

                    int lcid = int.Parse(parentItem.GetDataKeyValue("LCID").ToString());
                    CultureInfo preInvoiceCulture = new CultureInfo(lcid);

                    if (Decimal.TryParse(txtRate.Text, System.Globalization.NumberStyles.Currency, preInvoiceCulture, out rate))
                    {
                        decimal oldRate = 0;
                        oldRate = decimal.Parse(hidOldRate.Value);

                        // Only apply the rate change if the rate has altered.
                        if (oldRate != rate)
                        {
                            int itemID = int.Parse(txtRate.Attributes["ItemID"]);

                            // Update the rate (depends on item type - depends on invoice type)
                            int preInvoiceID = int.Parse(parentItem.GetDataKeyValue("PreInvoiceID").ToString());
                            eInvoiceType invoiceType = (eInvoiceType)int.Parse(parentItem.GetDataKeyValue("InvoiceTypeID").ToString());
                            CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                            Facade.IExchangeRates facER = new Facade.ExchangeRates();

                            switch (invoiceType)
                            {
                                case eInvoiceType.ClientInvoicing:
                                    Facade.IOrder facOrder = new Facade.Order();
                                    Entities.Order order = facOrder.GetForOrderID(itemID);
                                    order.ForeignRate = rate;

                                    if (order.LCID != culture.LCID)
                                    {
                                        BusinessLogicLayer.IExchangeRates blER = new BusinessLogicLayer.ExchangeRates();
                                        BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(order.LCID, order.CollectionDateTime);
                                        order.Rate = currencyConverter.ConvertToLocal(order.ForeignRate);
                                    }
                                    else
                                        order.Rate = decimal.Round(order.ForeignRate, 4, MidpointRounding.AwayFromZero);

                                    //if (order.OrderGroupID > 0)
                                    //facPreInvoice.UpdateOrderGroupRate(preInvoiceID, order.OrderGroupID, order.ForeignRate, order.Rate, userName);
                                    //else
                                   
                                    facPreInvoice.UpdateOrderRate(preInvoiceID, itemID, order.ForeignRate, order.Rate, userName);

                                    break;
                                case eInvoiceType.SubContractorSelfBill:
                                case eInvoiceType.SubContract:
                                    // Needs to be Amended For Currency : TL 12/05/08;
                                    Facade.IJobSubContractor facJSC = new Facade.Job();
                                    Entities.JobSubContractor jobSubContractor = facJSC.GetSubContractorForJobSubContractId(itemID);
                                    jobSubContractor.ForeignRate = rate;

                                    if (jobSubContractor.LCID != culture.LCID)
                                        jobSubContractor.Rate = facER.GetConvertedRate((int)jobSubContractor.ExchangeRateID, jobSubContractor.ForeignRate);
                                    else
                                        jobSubContractor.Rate = decimal.Round(jobSubContractor.ForeignRate, 4, MidpointRounding.AwayFromZero);

                                    facPreInvoice.UpdateJobSubContractRate(preInvoiceID, itemID, jobSubContractor.ForeignRate, jobSubContractor.Rate, jobSubContractor.ExchangeRateID, userName);
                                    break;
                                default:
                                    throw new NotSupportedException("You can not alter item rates on invoices of type " + invoiceType.ToString());
                            }

                            this.lblSavedMessage.Text = string.Format("Your changes were saved at {0}", DateTime.Now.ToLongTimeString());
                        }
                    }

                    e.Item.OwnerTableView.Rebind();
                    gvPreInvoices.MasterTableView.Rebind();
                    StayOpen = parentItem.ItemIndex;
                    break;

                #endregion
            }
        }

        void gvPreInvoices_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            string tableName = e.Item.OwnerTableView.Name;

            if (tableName == PreInvoiceTableName)
            {
                if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.EditItem)
                {
                    DataRowView drv = (DataRowView)e.Item.DataItem;

                    HtmlInputRadioButton rdoDoNothing = e.Item.FindControl("rdoDoNothing") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoReject = e.Item.FindControl("rdoReject") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoRegenerate = e.Item.FindControl("rdoRegenerate") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApprove = e.Item.FindControl("rdoApprove") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApproveAndPost = e.Item.FindControl("rdoApproveAndPost") as HtmlInputRadioButton;
                    DropDownList cboTaxRate = e.Item.FindControl("cboTaxRate") as DropDownList;
                    LinkButton lnkChangeClientReference = (LinkButton)e.Item.FindControl("lnkChangeClientReference");
                    TextBox txtClientReference = (TextBox)e.Item.FindControl("txtClientReference");
                    LinkButton lnkSaveClientReference = (LinkButton)e.Item.FindControl("lnkSaveClientReference");
                    LinkButton lnkCancelClientReference = (LinkButton)e.Item.FindControl("lnkCancelClientReference");

                    LinkButton lnkChangePurchaseOrderReference = (LinkButton)e.Item.FindControl("lnkChangePurchaseOrderReference");
                    TextBox txtPurchaseOrderReference = (TextBox)e.Item.FindControl("txtPurchaseOrderReference");
                    LinkButton lnkSavePurchaseOrderReference = (LinkButton)e.Item.FindControl("lnkSavePurchaseOrderReference");
                    LinkButton lnkCancelPurchaseOrderReference = (LinkButton)e.Item.FindControl("lnkCancelPurchaseOrderReference");

                    Label lblNetAmount = e.Item.FindControl("lblNetAmount") as Label;
                    Label lblExtraAmount = e.Item.FindControl("lblExtraAmount") as Label;
                    Label lblFuelSurchargeAmount = e.Item.FindControl("lblFuelSurchargeAmount") as Label;
                    Label lblTaxAmount = e.Item.FindControl("lblTaxAmount") as Label;
                    Label lblTotalAmount = e.Item.FindControl("lblTotalAmount") as Label;

                    HtmlInputCheckBox chkUseHeadedPaper = e.Item.FindControl("chkUseHeadedPaper") as HtmlInputCheckBox;

                    rdoDoNothing.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoReject.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoRegenerate.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoApprove.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoApproveAndPost.Name += "_" + e.Item.ItemIndex.ToString();

                    if (this.dsTaxRates == null)
                    {
                        Orchestrator.Facade.Invoice facInvoice = new Orchestrator.Facade.Invoice();
                        DataSet taxRates = facInvoice.GetTaxRates((DateTime)drv["InvoiceDate"]);
                        this.dsTaxRates = taxRates;
                    }

                    cboTaxRate.DataTextField = "Description";
                    cboTaxRate.DataValueField = "VatNo";
                    cboTaxRate.DataSource = this.dsTaxRates;
                    cboTaxRate.DataBind();
                    cboTaxRate.ClearSelection();
                    int taxRateID = 0;
                    int.TryParse(drv["TaxRateID"].ToString(), out taxRateID);
                    if (cboTaxRate.Items.FindByValue(taxRateID.ToString()) == null)
                    {
                        ListItem newItem = new ListItem("New Standard", taxRateID.ToString());
                        cboTaxRate.Items.Add(newItem);
                    }

                    cboTaxRate.Items.FindByValue(taxRateID.ToString()).Selected = true;

                    if (drv["ClientInvoiceReference"] != DBNull.Value)
                        txtClientReference.Text = lnkChangeClientReference.Text = (string)drv["ClientInvoiceReference"];
                    else
                    {
                        txtClientReference.Text = string.Empty;
                        lnkChangeClientReference.Text = "none provided";
                    }

                    if (drv["PurchaseOrderReference"] != DBNull.Value)
                        txtPurchaseOrderReference.Text = lnkChangePurchaseOrderReference.Text = (string)drv["PurchaseOrderReference"];
                    else
                    {
                        txtPurchaseOrderReference.Text = string.Empty;
                        lnkChangePurchaseOrderReference.Text = "none provided";
                    }

                    txtPurchaseOrderReference.Visible = lnkSavePurchaseOrderReference.Visible = lnkCancelPurchaseOrderReference.Visible = e.Item.Edit;
                    lnkChangePurchaseOrderReference.Visible = !e.Item.Edit;

                    txtClientReference.Visible = lnkSaveClientReference.Visible = lnkCancelClientReference.Visible = e.Item.Edit;
                    lnkChangeClientReference.Visible = !e.Item.Edit;

                    if ((bool)drv["RequiresRegeneration"])
                    {
                        rdoApprove.Visible = false;
                        rdoApproveAndPost.Visible = false;
                        rdoRegenerate.Checked = true;
                    }
                    //else
                    //    rdoApprove.Checked = true;

                    int lcID = (int)drv["LCID"];
                    CultureInfo culture = new CultureInfo(lcID);

                    lblNetAmount.Text = ((decimal)drv["ForeignNetAmount"]).ToString("C", culture);
                    lblExtraAmount.Text = ((decimal)drv["ForeignExtraAmount"]).ToString("C", culture);
                    lblFuelSurchargeAmount.Text = ((decimal)drv["ForeignFuelSurchargeAmount"]).ToString("C", culture);
                    lblTaxAmount.Text = ((decimal)drv["ForeignTaxAmount"]).ToString("C", culture);
                    lblTotalAmount.Text = ((decimal)drv["ForeignTotalAmount"]).ToString("C", culture);

                    chkUseHeadedPaper.Checked = Orchestrator.Globals.Configuration.UseHeadedPaper;
                }
                else if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Header)
                {
                    RadioButton rdoDoNothing = e.Item.FindControl("rdoDoNothing") as RadioButton;
                    RadioButton rdoReject = e.Item.FindControl("rdoReject") as RadioButton;
                    RadioButton rdoRegenerate = e.Item.FindControl("rdoRegenerate") as RadioButton;
                    RadioButton rdoApprove = e.Item.FindControl("rdoApprove") as RadioButton;
                    RadioButton rdoApproveAndPost = e.Item.FindControl("rdoApproveAndPost") as RadioButton;

                    rdoDoNothing.GroupName += "_Header";
                    rdoReject.GroupName += "_Header";
                    rdoRegenerate.GroupName += "_Header";
                    rdoApprove.GroupName += "_Header";
                    rdoApproveAndPost.GroupName += "_Header";

                    AddReaction(rdoDoNothing, "spnDoNothing");
                    AddReaction(rdoReject, "spnReject");
                    AddReaction(rdoRegenerate, "spnRegenerate");
                    AddReaction(rdoApprove, "spnApprove");
                    AddReaction(rdoApproveAndPost, "spnApproveAndPost");

                    //rdoApprove.Checked = true;
                }
            }
            else if (tableName == PreInvoiceItemTableName)
            {
                if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.EditItem)
                {
                    HiddenField hidOldRate = e.Item.FindControl("hidOldRate") as HiddenField;
                    TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;
                    Label lblPalletSpaces = e.Item.FindControl("lblPalletSpaces") as Label;
                    //LinkButton lnkChangeRate = e.Item.FindControl("lnkChangeRate") as LinkButton;
                    //LinkButton lnkCancelRate = e.Item.FindControl("lnkCancelRate") as LinkButton;
                    //LinkButton lnkSaveRate = e.Item.FindControl("lnkSaveRate") as LinkButton;

                    DataRowView drv = e.Item.DataItem as DataRowView;
                    int itemID = (int)drv["ItemID"];
                    txtRate.Attributes.Add("ItemID", ((int)drv["ItemID"]).ToString());

                    int lcID = (int)drv["LCID"];
                    CultureInfo culture = new CultureInfo(lcID);
                    txtRate.Text = ((decimal)drv["ForeignRate"]).ToString("C", culture);
                    lblPalletSpaces.Text = ((decimal)drv["PalletSpaces"]).ToString("F2");
                    hidOldRate.Value = ((decimal)drv["ForeignRate"]).ToString("F2");
                    //txtRate.Visible = e.Item.Edit;
                    //lnkChangeRate.Visible = !e.Item.Edit;

                    // If the item is part of a group only allow the rate to be changed on the first item.
                    //int groupID = (int)drv["GroupID"];
                    //if (!e.Item.Edit && groupID > 0)
                    //{
                    //    // Is there any item with the same group id but a lower item id?
                    //    bool groupAlreadyRendered =
                    //        drv.Row.Table.Select(string.Format("GroupID = {0} AND ItemID < {1}", groupID, itemID)).
                    //            Length > 0;
                    //    //lnkChangeRate.Visible = !groupAlreadyRendered;
                    //}
                }
            }
        }

        void gvPreInvoices_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IWorkflowPreInvoice facWorkflowPreInvoice = new Facade.PreInvoice();
            gvPreInvoices.DataSource = facWorkflowPreInvoice.GetForWorkflowInstanceID(WorkflowInstanceID);
        }

        #endregion

        private void SetUseHeadedPaperParameter(int preInvoiceID, bool useHeadedPaper)
        {
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            facPreInvoice.UpdateUseHeadedPaper(preInvoiceID, useHeadedPaper, ((Entities.CustomPrincipal)Page.User).UserName);
        }
        #endregion
    }
}
