using System;
using System.Data;
using System.Data.Linq;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using Orchestrator;
using Order = Orchestrator.Entities.Order;

namespace Orchestrator.WebUI.Client
{
    public partial class ClientOrderProfile : Orchestrator.Base.BasePage
    {

        #region Page Properties

        private DataSet dsOrders = null;
        private CultureInfo _currentCulture = null;
        private int? _exchangeRateID = -1;

        /// <summary>
        /// Gets or sets the order ID.
        /// This will attempt to get the Order ID from the Viewstate first otherwise it will 
        /// attempt to retrieve this from the quesrystring.
        /// </summary>
        /// <value>The order ID.</value>
        /// 

        protected int OrderID
        {
            get
            {
                int _orderID = 0;
                if (this.ViewState["_orderID"] != null)
                    _orderID = (int)this.ViewState["_orderID"];

                if (_orderID == 0 && !string.IsNullOrEmpty(Request.QueryString["oID"]))
                    _orderID = int.Parse(Request.QueryString["oID"]);

                return _orderID;
            }
            set { this.ViewState["_orderID"] = value; }
        }

        protected CultureInfo CurrentCulture
        {
            get { return _currentCulture == null ? new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture) : _currentCulture; }
            set { _currentCulture = value; }
        }

        protected int ExchangeRateForExtra
        {
            get { return _exchangeRateID == null ? -1 : (int)_exchangeRateID; }
        }

        protected int? ExchangeRateID
        {
            get { return _exchangeRateID; }
            set { _exchangeRateID = value; }
        }

        private const string rateTemplate = @"<div style=""float:left;"">{0}</div> <div style=""margin-left:10px; float:left;"">( {1} )</div>";

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.OrderID = int.Parse(Request.QueryString["oID"]);
                ShowOrder();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnCreateDeliveryNote2.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnPIL.Click += new EventHandler(btnPIL_Click);
            this.btnPIL2.Click += new EventHandler(btnPIL_Click);

            this.grdExtras.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdExtras_NeedDataSource);
            this.grdExtras.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdExtras_ItemDataBound);

            this.RadGridForSubby.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(RadGridForSubby_ItemDataBound);
            this.RadGridForSubby.NeedDataSource += new GridNeedDataSourceEventHandler(RadGridForSubby_NeedDataSource);
        }

        void RadGridForSubby_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;
                if (drv != null)
                {
                    HtmlAnchor lnkViewInvoice = e.Item.FindControl("lnkViewInvoice") as HtmlAnchor;      //setup a HTML Anchor

                    if (lnkViewInvoice != null && drv["PDFLocation"] != DBNull.Value)
                        lnkViewInvoice.HRef = Orchestrator.Globals.Configuration.WebServer + (string)drv["PDFLocation"]; //selfbill

                    if (Orchestrator.Globals.Configuration.MultiCurrency)
                    {
                        Label lblForeignRate = e.Item.FindControl("lblForeignRate") as Label;

                        if (lblForeignRate != null && drv["ForeignRate"] != DBNull.Value)
                            lblForeignRate.Text = ((decimal)drv["ForeignRate"]).ToString("C", CurrentCulture);
                    }
                }
            }
        }

        void RadGridForSubby_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            DataSet subbies = facOrder.GetSubcontractorDetailsFrom(OrderID);
            RadGridForSubby.DataSource = subbies;
        }

        void grdExtras_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                GridViewRow grv = e.Item.DataItem as GridViewRow;
                Label lblExtraAmount = e.Item.FindControl("lblExtraAmount") as Label;
                Label lblExtraForeignAmount = e.Item.FindControl("lblExtraForeignAmount") as Label;
                Telerik.Web.UI.GridDataItem dgi = e.Item as Telerik.Web.UI.GridDataItem;

                if ((int)((DataRowView)dgi.DataItem)["ExtraTypeId"] == (int)eExtraType.Custom)
                {
                    Telerik.Web.UI.GridDataItem gdi = e.Item as Telerik.Web.UI.GridDataItem;
                    e.Item.OwnerTableView.Columns[1].Visible = true;
                }

                if (lblExtraForeignAmount != null && (((DataRowView)dgi.DataItem)["ForeignAmount"]) != DBNull.Value)
                    lblExtraForeignAmount.Text = ((decimal)((DataRowView)dgi.DataItem)["ForeignAmount"]).ToString("C", CurrentCulture);

                if (lblExtraAmount != null)
                    lblExtraAmount.Text = ((decimal)((DataRowView)dgi.DataItem)["ExtraAmount"]).ToString("C");
            }
        }

        void grdExtras_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Orchestrator.Facade.IOrderExtra facOrder = new Orchestrator.Facade.Order();
            DataSet dsExtras = facOrder.GetExtrasForOrderID(OrderID);

            DataRow[] drs = dsExtras.Tables[0].Select("ExtraAppliesToID=1");

            grdExtras.DataSource = drs;
        }

        #endregion

        #region Display Order

        private void ShowOrder()
        {
            Orchestrator.Entities.Order o = GetOrder();
            DisplayOrder(o);

            // Check to see if the order is being invoiced.
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            bool orderBeingInvoiced = facOrder.IsOrderBeingInvoiced(o.OrderID);

            if (orderBeingInvoiced)
            {
                int invoiceId = facOrder.ClientInvoiceID(o.OrderID);
            }
            this.lblCustomerOrderNumberText.Text = Globals.Configuration.SystemLoadNumberText;
            this.lblDeliveryOrderNumberText.Text = Globals.Configuration.SystemDocketNumberText;
        }

        #endregion

        #region Entity Handling

        private Orchestrator.Entities.Order GetOrder()
        {
            Orchestrator.Entities.Order order = null;

            if (this.ViewState["_order"] == null)
            {
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                order = facOrder.GetForOrderID(OrderID);
                this.ViewState["_order"] = order;
            }
            else
                order = this.ViewState["_order"] as Orchestrator.Entities.Order;

            return order;
        }

        private void DisplayOrder(Orchestrator.Entities.Order order)
        {
            Orchestrator.Facade.IOrganisation facOrg = new Orchestrator.Facade.Organisation();
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            Orchestrator.Facade.IPOD facPOD = new Orchestrator.Facade.POD();

            Orchestrator.Facade.IOrder facOrd = new Orchestrator.Facade.Order();
            order.ClientInvoiceID = facOrd.ClientInvoiceID(OrderID);
            Orchestrator.Facade.Organisation facOrgD = new Orchestrator.Facade.Organisation();

            Orchestrator.Entities.Point collectionPoint = facPoint.GetPointForPointId(order.CollectionPointID);
            Orchestrator.Entities.Point deliveryPoint = facPoint.GetPointForPointId(order.DeliveryPointID);
            Orchestrator.Entities.POD scannedPOD = facPOD.GetForOrderID(order.OrderID);
            Orchestrator.Entities.Scan scannedBookingForm = null;

            CurrentCulture = new CultureInfo(order.LCID);
            ExchangeRateID = order.ExchangeRateID;

            lblOrderHeading.Text = string.Format("Order {0}", order.OrderID);

            this.lblOrderStatus.Text = order.OrderStatus.ToString();

            if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
            {
                this.btnPIL.Visible = false;
                this.btnCreateDeliveryNote.Visible = false;
                this.btnCreateDeliveryNote2.Visible = false;
                this.btnPIL2.Visible = false;
                plcBooking.Visible = false;

                plcPOD.Visible = false;

                // get cost for subby and display as rate. (The rate the subby is being paid)
                Facade.IJobSubContractor jobSubContractor = new Facade.Job();
                if (order != null && order.JobSubContractID > 0)
                {
                    Entities.JobSubContractor js = jobSubContractor.GetSubContractorForJobSubContractId(order.JobSubContractID);
                    CultureInfo subbyCulture = new CultureInfo(js.LCID);

                    if (Orchestrator.Globals.Configuration.MultiCurrency)
                    {
                        this.lblRate.Text = string.Format(rateTemplate, js.ForeignRate.ToString("C", subbyCulture), js.Rate.ToString("C"));
                    }
                    else
                    {
                        this.lblRate.Text = js.Rate.ToString("C", subbyCulture);
                    }
                }

                this.tblSubbyRate.Visible = true;
                this.trRate.Visible = false;
                this.trInvoiceId.Visible = false;
            }
            else
            {
                this.tblSubbyRate.Visible = false;
                //If the order has a scanned Booking Form get it so that
                //a link to it can be displayed
                if (order.BookingFormScannedFormId != null)
                {
                    Orchestrator.Facade.Form facBF = new Orchestrator.Facade.Form();
                    scannedBookingForm = facBF.GetForScannedFormId(order.BookingFormScannedFormId.Value);
                }

                //this.lblOrderID.Text = order.OrderID.ToString();
                this.lblOrderStatus.Text = order.OrderStatus.ToString().Replace("_", " ");

                if (scannedBookingForm != null)
                {
                    hlBookingFormLink.Visible = true;
                    hlBookingFormLink.NavigateUrl = scannedBookingForm.ScannedFormPDF.Trim();

                    aScanBookingForm.InnerHtml = "| Re-Scan";
                    aScanBookingForm.HRef = @"javascript:ReDoBookingForm(" + scannedBookingForm.ScannedFormId + "," + order.OrderID.ToString() + ");";

                }
                else
                {
                    hlBookingFormLink.Visible = false;
                    aScanBookingForm.InnerHtml = "Scan";
                    aScanBookingForm.HRef = @"javascript:NewBookingForm(" + order.OrderID.ToString() + ");";
                }

                plcPOD.Visible = false;

                if (Orchestrator.Globals.Configuration.MultiCurrency)
                    this.lblRate.Text = string.Format(rateTemplate, order.ForeignRate.ToString("C", CurrentCulture), order.Rate.ToString("C"));
                else
                    this.lblRate.Text = order.ForeignRate.ToString("C", CurrentCulture);

                trRate.Visible = !order.IsInGroup;

                if (order.ClientInvoiceID <= 0)
                {
                    lblInvoiceNumber.Text = "None Assigned";
                }
                else
                {
                    lblInvoiceNumber.Text = order.ClientInvoiceID.ToString();
                    string PDFLink = order.PDFLocation.ToString();
                    lblInvoiceNumber.NavigateUrl = Orchestrator.Globals.Configuration.WebServer + PDFLink;
                }

            }
            this.lblLoadNumber.Text = order.CustomerOrderNumber;
            this.lblDeliveryOrderNo.Text = order.DeliveryOrderNumber;

            this.lblCollectionPoint.Text = collectionPoint.Address.ToString();
            this.lblDeliverTo.Text = deliveryPoint.Address.ToString();

            this.lblCollectDateTime.Text = (order.CollectionIsAnytime == true ? (order.CollectionDateTime.ToString("dd/MM/yy") + " AnyTime") : (order.CollectionDateTime.ToString("dd/MM/yy HH:mm")));
            this.lblDeliveryDateTime.Text = (order.DeliveryIsAnytime == true ? (order.DeliveryDateTime.ToString("dd/MM/yy") + " AnyTime") : (order.DeliveryDateTime.ToString("dd/MM/yy HH:mm"))) + order.DeliveryAnnotation;

            this.lblPallets.Text = order.NoPallets.ToString() + " " + Orchestrator.Facade.PalletType.GetForPalletTypeId(order.PalletTypeID).Description;
            this.lblPalletSpaces.Text = order.PalletSpaces.ToString("0.##");
            this.lblGoodsType.Text = Orchestrator.Facade.GoodsType.GetForGoodsTypeId(order.GoodsTypeID).Description;
            this.lblWeight.Text = Convert.ToInt32(order.Weight).ToString() + " " + Orchestrator.Facade.WeightType.GetForWeightTypeId(order.WeightTypeID).ShortCode;

            this.repReferences.DataSource = order.OrderReferences;
            this.repReferences.DataBind();

            this.lblCartons.Text = order.Cases.ToString();

            if (order.Notes == null || order.Notes.Length == 0)
                this.lblNotes.Text = "&#160;";
            else
                this.lblNotes.Text = order.Notes;

            if (order.CreateDateTime != DateTime.MinValue)
                lblCreated.Text = order.CreatedBy + " on " + order.CreateDateTime.ToString("dd/MM/yy HH:mm");
            lblOrderServiceLevel.Text = order.OrderServiceLevel;

            if (order.BusinessTypeID > 0)
            {
                Orchestrator.Facade.IBusinessType facBusinessType = new Orchestrator.Facade.BusinessType();
                Orchestrator.Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(order.BusinessTypeID);
                lblBusinessType.Text = businessType.Description;
            }
            else
                lblBusinessType.Text = "Not Set";

            plcCancellation.Visible = order.OrderStatus == eOrderStatus.Cancelled;
            if (order.OrderStatus == eOrderStatus.Cancelled)
            {
                lblCancellationReason.Text = order.CancellationReason;
                lblCancelledBy.Text = order.CancelledBy;
                lblCancelledAt.Text = order.CancelledAt.ToString("dd/MM/yy HH:mm");
            }

        }

        #endregion

        #region Button Event Handlers

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (OrderID > 0)
            {
                dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(OrderID.ToString());

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DeliveryNote;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDelivery;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            }
        }

        void btnPIL_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (OrderID > 0)
            {
                #region Pop-up Report
                eReportType reportType = eReportType.PIL;
                dsPIL = facLoadOrder.GetPILData(OrderID.ToString());
                DataView dvPIL;

                if ((bool)dsPIL.Tables[0].Rows[0]["IsPalletNetwork"])
                {
                    reportType = Globals.Configuration.PalletNetworkLabelID; ;

                    //Need to duplicate the rows for the Pallteforce labels
                    dsPIL.Tables[0].Merge(dsPIL.Tables[0].Copy(), true);
                    dvPIL = new DataView(dsPIL.Tables[0], string.Empty, "OrderId, PalletCount", DataViewRowState.CurrentRows);
                }
                else
                {
                    dvPIL = new DataView(dsPIL.Tables[0]);
                }
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

        #endregion
    }
}
