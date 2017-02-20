using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using Orchestrator.Facade;
using Orchestrator.WebUI;
using Orchestrator.Entities;

using Point = Orchestrator.Entities.Point;

using Telerik.Web.UI;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Orchestrator.WebUI.Client
{
    public partial class ClientManageOrder : Orchestrator.Base.BasePage
    {
        private Orchestrator.Entities.OrganisationReferenceCollection _clientReferences = null;

        private int _clientIdentityId = 0;
        private List<DateTime> InvalidDates = null;
        private const eOrderInstructionType _orderInstruction = eOrderInstructionType.CollectAndDeliver;

        #region Page Properties

        protected int ClientIdentityID
        {
            get { return _clientIdentityId; }
        }

        protected int OrderInstructionID
        {
            get { return (int)_orderInstruction; }
        }

        protected bool IsUpdate
        {
            get { return this.ViewState["_isUpdate"] == null ? false : (bool)this.ViewState["_isUpdate"]; }
            set
            {
                this.ViewState["_isUpdate"] = value;
                pageIsUpdate.Value = value.ToString();
            }
        }

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

        protected int BusinessTypeID
        {
            get { return (int?)ViewState["BusinessTypeID"] ?? 0; }
            set { ViewState["BusinessTypeID"] = value; }
        }

        protected Orchestrator.Entities.Order SavedOrder
        {
            get { return (Orchestrator.Entities.Order)this.ViewState["_order"]; }
            set { this.ViewState["_order"] = value; }
        }

        protected bool IsRateEditable
        {
            get { return Orchestrator.Globals.Configuration.ClientRateFieldIsEditable; }
        }

        protected bool DisplaySurcharges
        {
            get { return Orchestrator.Globals.Configuration.DisplaySurchargesOnClientAddOrder; }
        }

        #endregion

        #region OnInit/Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);

            GetClientId();

            OrderID = 0;

            try
            { OrderID = Convert.ToInt32(Request.QueryString["OrderId"]); }
            catch
            { OrderID = 0; }

            if (!IsPostBack)
            {
                PopulateStaticControls();

                ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
                ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";

                ConfigureClientReferences();

                litSetFocus.Controls.Clear();
                litSetFocus.Text = string.Empty;
                SetFocus(txtLoadNumber.ClientID);

                if (OrderID > 0)
                {
                    #region Update

                    // Populate order fields.
                    Facade.IOrder facOrder = new Facade.Order();
                    this.SavedOrder = facOrder.GetForOrderID(OrderID);
                    this.BusinessTypeID = this.SavedOrder.BusinessTypeID;

                    ShowOrHidePalletNetworkFields();

                    // this is an existing order, so see if the business type is a pallet network.
                    Facade.BusinessType facBusinessType = new Orchestrator.Facade.BusinessType();
                    Entities.BusinessType orderBusinessType = facBusinessType.GetForBusinessTypeID(this.SavedOrder.BusinessTypeID);

                    if (orderBusinessType.IsPalletNetwork)
                    {
                        this.trPalletNetworkFields.Visible = true;
                        this.trPalletSpaces.Visible = false;
                        this.trNoPallets.Visible = false;
                    }

                    // This is an existing order
                    _isUpdate = true;
                    this.tblRowOrderId.Visible = true;
                    this.tblRowCancelOrder.Visible = true;

                    this.btnSubmit.Visible = false;
                    this.btnAddAndReset.Visible = false;
                    this.btnUpdate.Visible = true;
                    this.btnBack.Visible = true;

                    PopulateFieldsFromOrder();

                    #endregion
                }
                else
                {
                    ShowOrHidePalletNetworkFields();
                    PageLoadNewOrder();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _clientIdentityId = 0;

            this.repReferences.ItemDataBound += new RepeaterItemEventHandler(repReferences_ItemDataBound);

            this.btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);
            this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
            this.btnAddAndReset.Click += new EventHandler(btnAddAndReset_Click);

            this.cfvDelivery.ServerValidate += new ServerValidateEventHandler(cfvDelivery_ServerValidate);
            this.cfvNoPallets.ServerValidate += new ServerValidateEventHandler(cfvNoPallets_ServerValidate);
            this.cfvOrderDetailValidation.ServerValidate += new ServerValidateEventHandler(cfvOrderDetailValidation_ServerValidate);
        }

        private void ShowOrHidePalletNetworkFields()
        {
            this.trPalletNetworkFields.Visible = false;
            this.trPalletSpaces.Visible = true;
            this.trNoPallets.Visible = true;
            this.hidShowPalletNetworkFields.Value = "false";

            int businessTypeId = 0;

            if (OrderID > 0)
            {
                businessTypeId = this.SavedOrder.BusinessTypeID;
            }
            else
            {
                var defaultBusinessTypeId =
                    (
                        from od in EF.DataContext.Current.OrganisationDefaultsSet
                        where od.IdentityId == _clientIdentityId
                        select od.DefaultBusinessTypeID
                    ).FirstOrDefault();

                if (defaultBusinessTypeId != null)
                    businessTypeId = defaultBusinessTypeId.Value;
            }

            if (businessTypeId > 0)
            {
                Facade.BusinessType facBusinessType = new Orchestrator.Facade.BusinessType();
                Entities.BusinessType bt = facBusinessType.GetForBusinessTypeID(businessTypeId);

                if (bt.IsPalletNetwork)
                {
                    this.trPalletNetworkFields.Visible = true;
                    hidShowPalletNetworkFields.Value = "true";
                    this.trPalletSpaces.Visible = false;
                    this.trNoPallets.Visible = false;
                }
            }
        }

        private void LoadInvalidDates()
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();

            // If the list of invalid dates has not been set then create it.
            if (InvalidDates == null)
            {
                InvalidDates = new List<DateTime>();

                DateTime startDate = DateTime.Today.AddMonths(-2);
                DateTime endDate = DateTime.Today.AddMonths(10);

                DataSet ds = facOrg.GetAllPublicHolidays(startDate, endDate);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        InvalidDates.Add((DateTime)dr["PublicHolidayDate"]);

                //List of weekend dates from 2 months prior to todays date, to 10 months into the future.
                while (startDate != endDate)
                {
                    if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                        InvalidDates.Add(startDate);

                    startDate = startDate.AddDays(1);
                }
            }

            lvInvalidDates.DataSource = InvalidDates;
            lvInvalidDates.DataBind();
        }

        private void LoadDeliveryDates()
        {
            //// Set the Default dates
            dteCollectionFromDate.SelectedDate = DateTime.Today;
            dteCollectionByDate.SelectedDate = DateTime.Today;
            dteDeliveryByDate.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryFromDate.SelectedDate = DateTime.Today.AddDays(1);

            // Find the selected service level 
            RadComboBoxItem li = cboService.SelectedItem;
            if (li != null)
            {
                //Check for weekend and public holiday dates, load if not loaded.
                if (InvalidDates != null)
                    LoadInvalidDates();

                //Find the Number of days for the specified service level.
                Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
                DataSet dsServices = facOrder.GetAll();
                DataRow dr = dsServices.Tables[0].Rows.Cast<DataRow>().FirstOrDefault(cr => (int)cr["OrderServiceLevelID"] == int.Parse(li.Value));

                // If the number of days has been set, proceed.
                if (dr != null && dr["NoOfDays"] != DBNull.Value)
                {
                    int noOfDays = (int)dr["NoOfDays"];

                    // Get the latest collection date.
                    DateTime newDeliveryDate = dteCollectionByDate.SelectedDate.Value.AddDays(noOfDays);

                    // If the no of days added to the collection date is a weekend or public holiday, keep going until its neither.
                    while (InvalidDates.Exists(ivd => ivd == newDeliveryDate))
                    {
                        noOfDays++;
                        newDeliveryDate = dteCollectionByDate.SelectedDate.Value.AddDays(noOfDays);
                    }

                    // Set BOTH the delivery from and by date with the new delivery DATE.
                    dteDeliveryFromDate.SelectedDate = newDeliveryDate;
                    dteDeliveryByDate.SelectedDate = newDeliveryDate;
                }
            }
        }

        private bool _isUpdate = false;
        private void PopulateFieldsFromOrder()
        {
            lblOrderId.Text = this.SavedOrder.OrderID.ToString();

            Orchestrator.Entities.Point point = null;
            Orchestrator.Entities.Organisation org = null;
            Orchestrator.Facade.IOrganisation facOrg = new Orchestrator.Facade.Organisation();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            // Set the Customer Identity
            org = facOrg.GetForIdentityId(this.SavedOrder.CustomerIdentityID);

            // Set the Load Number
            this.txtLoadNumber.Text = this.SavedOrder.CustomerOrderNumber;

            // Set the Client Order references
            Orchestrator.Facade.IOrganisationReference facOrganisationReference = new Orchestrator.Facade.Organisation();
            Orchestrator.Entities.OrganisationReferenceCollection _clientReferences = null;
            _clientReferences = facOrganisationReference.GetReferencesForOrganisationIdentityId(ClientIdentityID, true);
            if (_clientReferences == null)
                _clientReferences = new Entities.OrganisationReferenceCollection();

            // Ensure each of the references in the client reference collection.
            foreach (Orchestrator.Entities.OrderReference reference in this.SavedOrder.OrderReferences)
                if (_clientReferences.FindByReferenceId(reference.OrganisationReference.OrganisationReferenceId) == null)
                    _clientReferences.Add(reference.OrganisationReference);

            if (_clientReferences != null)
            {
                repReferences.DataSource = _clientReferences;
                repReferences.DataBind();
            }

            //Set the Service Level
            cboService.ClearSelection();
            cboService.FindItemByValue(this.SavedOrder.OrderServiceLevelID.ToString()).Selected = true;

            // Set the delivery Order Number
            this.txtDeliveryOrderNumber.Text = this.SavedOrder.DeliveryOrderNumber;

            /*
             * Basic rules for collection and delivery:
             * ----------------------------------------
             * If the from and by dates are exactly the same then the timing method is "Timed Booking".
             * If the from and by date times are midnight to 23:59 AND the from and by dates are the same day then the timing method is "Anytime" (note, this should be determined using the Anytime flag on the order entity).
             * If neither of the above apply, the timing method is booking window.
             * 
             */

            ///// COLLECTION DATES //////

            // Set the Collection Date and Is Anytime   

            dteCollectionFromDate.SelectedDate = this.SavedOrder.CollectionDateTime;
            dteCollectionByDate.SelectedDate = this.SavedOrder.CollectionByDateTime;
            dteCollectionFromTime.SelectedDate = this.SavedOrder.CollectionDateTime;
            dteCollectionByTime.SelectedDate = this.SavedOrder.CollectionByDateTime;

            if (this.SavedOrder.CollectionIsAnytime)
            {
                // Anytime
                rdCollectionIsAnytime.Checked = true;
                rdCollectionTimedBooking.Checked = false;
                rdCollectionBookingWindow.Checked = false;

                dteCollectionFromTime.Enabled = false;
                hidCollectionTimingMethod.Value = "anytime";
            }
            else if (this.SavedOrder.CollectionDateTime == this.SavedOrder.CollectionByDateTime)
            {
                // Timed booking
                rdCollectionIsAnytime.Checked = false;
                rdCollectionTimedBooking.Checked = true;
                rdCollectionBookingWindow.Checked = false;

                hidCollectionTimingMethod.Value = "timed";
            }
            else
            {
                // Booking window
                rdCollectionIsAnytime.Checked = false;
                rdCollectionTimedBooking.Checked = false;
                rdCollectionBookingWindow.Checked = true;

                hidCollectionTimingMethod.Value = "window";
            }

            ////////////////////////

            ////// DELIVERY DATES ////////

            dteDeliveryFromDate.SelectedDate = this.SavedOrder.DeliveryFromDateTime;
            dteDeliveryByDate.SelectedDate = this.SavedOrder.DeliveryDateTime;
            dteDeliveryByTime.SelectedDate = this.SavedOrder.DeliveryDateTime;
            dteDeliveryFromTime.SelectedDate = this.SavedOrder.DeliveryFromDateTime;

            if (this.SavedOrder.DeliveryIsAnytime)
            {
                // Anytime
                rdDeliveryIsAnytime.Checked = true;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = false;

                dteDeliveryFromTime.Enabled = false;
                hidDeliveryTimingMethod.Value = "anytime";

            }
            else if (this.SavedOrder.DeliveryFromDateTime == this.SavedOrder.DeliveryDateTime)
            {
                // Timed booking
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = true;
                rdDeliveryBookingWindow.Checked = false;

                hidDeliveryTimingMethod.Value = "timed";
            }
            else
            {
                // Booking window
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = true;

                hidDeliveryTimingMethod.Value = "window";
            }

            // Set the Collection Point
            point = facPoint.GetPointForPointId(this.SavedOrder.CollectionPointID);
            this.ucCollectionPoint.SelectedPoint = point;
            if (this.SavedOrder.PlannedForCollection)
                this.ucCollectionPoint.CanChangePoint = false;
            else
                this.ucCollectionPoint.CanChangePoint = this.SavedOrder.CollectionPointID == this.SavedOrder.CollectionRunDeliveryPointID; // Can change the collection point if the order has not been collected.

            //Set the Delivery Point
            point = facPoint.GetPointForPointId(this.SavedOrder.DeliveryPointID);
            this.ucDeliveryPoint.SelectedPoint = point;
            this.ucDeliveryPoint.CanChangePoint = this.SavedOrder.OrderStatus < eOrderStatus.Delivered; // Can change delivery point if the order has not been planned for delivery.

            //Set the pallet Type
            this.cboPalletType.FindItemByValue(this.SavedOrder.PalletTypeID.ToString()).Selected = true;
            rntxtPalletSpaces.Text = this.SavedOrder.PalletSpaces.ToString();

            cboGoodsType.FindItemByValue(this.SavedOrder.GoodsTypeID.ToString()).Selected = true;

            //Set the Number of Pallets
            if (this.txtNoPallets.Visible)
            {
                this.txtNoPallets.Text = this.SavedOrder.NoPallets.ToString();
                this.txtWeight.Text = Convert.ToInt32(this.SavedOrder.Weight).ToString();
            }
            else
            {
                EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == this.SavedOrder.OrderID);

                rntxtFullPallets.Value = vigoOrder.FullPallets;
                rntxtHalfPallets.Value = vigoOrder.HalfPallets;
                rntxtQtrPallets.Value = vigoOrder.QtrPallets;
                rntxtOverSizePallets.Value = vigoOrder.OverPallets;
            }

            //set the Rate
            //Set the cuture of the Order Rate to that of the order
            this.rntRate.Culture = new CultureInfo(this.SavedOrder.LCID);
            this.rntRate.Value = (double)this.SavedOrder.ForeignRate;

            //se the Notes
            this.txtNotes.Text = this.SavedOrder.Notes;

        }

        #region Populate static controls

        private void PageLoadNewOrder()
        {
            // If the clients default business type is palletforce (i.e. IsPalletNetwork) then 
            // hide the No of pallets and pallet spaces rows and instead display full/over/half/qtr pallets.

            BusinessTypeID = this.GetDefaultBusinessTypeID();

            Facade.BusinessType facBusinessType = new Orchestrator.Facade.BusinessType();
            Entities.BusinessType busType = facBusinessType.GetForBusinessTypeID(BusinessTypeID);

            if (busType.IsPalletNetwork)
            {
                this.trPalletNetworkFields.Visible = true;
                this.trPalletSpaces.Visible = false;
                this.trNoPallets.Visible = false;
            }

            // Set the default service level
            var defaultServiceLevelId =
                (
                    from od in EF.DataContext.Current.OrganisationDefaultsSet
                    where od.IdentityId == _clientIdentityId
                    select od.DefaultServiceLevelID
                ).FirstOrDefault();

            if (defaultServiceLevelId > 0)
            {
                cboService.ClearSelection();
                RadComboBoxItem item = cboService.Items.FindItemByValue(defaultServiceLevelId.ToString());
                if (item != null)
                    item.Selected = true;
            }

            SetDefaultCollectionPoint();

            this.tblRowOrderId.Visible = false;
            this.tblRowCancelOrder.Visible = false;
            this.btnSubmit.Visible = true;
            this.btnAddAndReset.Visible = true;
            this.btnUpdate.Visible = false;
            this.btnBack.Visible = false;
        }

        private void PopulateStaticControls()
        {
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.Organisation org = facOrg.GetForIdentityId(ClientIdentityID);

            if (!string.IsNullOrEmpty(org.LoadNumberText))
                lblLoadNumber.Text = org.LoadNumberText;

            if (!string.IsNullOrEmpty(org.DocketNumberText))
                lblDocketNumber.Text = org.DocketNumberText;

            if (PageTitle != null)
                PageTitle.Text = "Order Details";

            LoadPalletTypes();
            LoadGoodsTypes();
            LoadServiceLevels();
            LoadInvalidDates();

            // Set the Default dates
            dteCollectionFromDate.SelectedDate = DateTime.Today;
            dteCollectionByDate.SelectedDate = DateTime.Today;
            dteDeliveryByDate.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryFromDate.SelectedDate = DateTime.Today.AddDays(1);

            this.SetDefaultCollectionBookingType();
            this.SetDefaultDeliveryBookingType();

            //Set Rate Is Editable
            rntRate.Enabled = IsRateEditable;
            rfvRate.Enabled = IsRateEditable;
        }

        private void ClearScreen()
        {
            PopulateStaticControls();

            txtLoadNumber.Text = "";
            //txtDeliveryAnnotation.Text = "";
            txtDeliveryOrderNumber.Text = "";
            txtNoPallets.Text = "";
            txtCases.Text = "";
            rntxtPalletSpaces.Text = "";
            rntRate.Text = "";
            txtNotes.Text = "";
            txtWeight.Text = "";
            ucCollectionPoint.Reset();
            ucDeliveryPoint.Reset();

            //Default to Collection Anytime
            rdCollectionBookingWindow.Checked = false;
            rdCollectionIsAnytime.Checked = true;
            rdCollectionTimedBooking.Checked = false;

            hidCollectionTimingMethod.Value = "anytime";
            dteCollectionFromTime.Enabled = false;

            dteCollectionFromDate.SelectedDate = DateTime.Today;
            dteCollectionFromTime.SelectedDate = DateTime.Today;
            dteCollectionByDate.SelectedDate = DateTime.Today;
            dteCollectionByTime.SelectedDate = DateTime.Today.Add(new TimeSpan(23, 59, 0));

            this.SetDefaultCollectionBookingType();

            //Default to Delivery Anytime
            rdDeliveryBookingWindow.Checked = false;
            rdDeliveryIsAnytime.Checked = true;
            rdDeliveryTimedBooking.Checked = false;

            hidDeliveryTimingMethod.Value = "anytime";
            dteDeliveryByTime.Enabled = false;

            dteDeliveryFromDate.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryFromTime.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryByDate.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryByTime.SelectedDate = DateTime.Today.AddDays(1).Add(new TimeSpan(23, 59, 0));

            this.SetDefaultDeliveryBookingType();

            // Clear any additional reference fields.
            foreach (RepeaterItem item in repReferences.Items)
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    //HtmlInputHidden ctl = (HtmlInputHidden)item.FindControl("hidOrganisationReferenceId");
                    TextBox ctl2 = (TextBox)item.FindControl("txtReferenceValue");
                    //ctl.Value = "";
                    ctl2.Text = "";
                }

            hidTariffRateDescription.Value = string.Empty;
            hidTariffRate.Value = string.Empty;
            hidSelectedSurchargeExtraTypeIDs.Value = string.Empty;

            rntxtFullPallets.Value = null;
            rntxtHalfPallets.Value = null;
            rntxtQtrPallets.Value = null;
            rntxtOverSizePallets.Value = null;

        }

        #endregion

        #region Load

        private void LoadPalletTypes()
        {
            DataSet dsPalletTypes = null;
            DataView vwActivePalletTypes = null;
            bool filterDefaultForActive = false;

            // Clear the pallet types
            cboPalletType.Items.Clear();

            if (!(ClientIdentityID == 0))
            {
                dsPalletTypes = Orchestrator.Facade.PalletType.GetAllPalletTypes(ClientIdentityID);
                vwActivePalletTypes = dsPalletTypes.DefaultViewManager.CreateDataView(dsPalletTypes.Tables[0]);

                // Count the active pallet types.
                int countActiveRows = 0;
                foreach (DataRow row in vwActivePalletTypes.Table.Rows)
                {
                    if (Convert.ToInt32(row["IsActive"]) == 1)
                    {
                        countActiveRows++;
                    }
                }

                if (countActiveRows == 0)
                {
                    // No pallet types are specified for the client - return all pallet types
                    vwActivePalletTypes.RowFilter = "";
                }
                else
                {
                    // The client has pallet types - return all the client pallet types
                    vwActivePalletTypes.RowFilter = "IsActive = 1";
                    filterDefaultForActive = true;
                }
            }
            else
            {
                dsPalletTypes = Orchestrator.Facade.PalletType.GetAllPalletTypes(0);
                vwActivePalletTypes = dsPalletTypes.DefaultViewManager.CreateDataView(dsPalletTypes.Tables[0]);
            }

            cboPalletType.DataSource = vwActivePalletTypes;
            cboPalletType.DataTextField = "Description";
            cboPalletType.DataValueField = "PalletTypeID";
            cboPalletType.DataBind();

            // Set the client default pallet type to display (if there is one)
            int defaultPalletTypeIndex = 0;
            bool foundDefaultPalletType = false;
            foreach (DataRow row in vwActivePalletTypes.Table.Rows)
            {
                // Note there is an assumption that if "IsDefault" is true then so is "IsActive"
                if (Convert.ToInt32(row["IsDefault"]) == 1)
                {
                    cboPalletType.Items[defaultPalletTypeIndex].Selected = true;
                    foundDefaultPalletType = true;
                    break;
                }
                else
                {
                    if (filterDefaultForActive && Convert.ToInt32(row["IsActive"]) == 1)
                    {
                        defaultPalletTypeIndex++;
                        // If filterDefaultForActive == true but IsActive == 0 then do not increment index
                    }
                    else if (filterDefaultForActive == false)
                    {
                        defaultPalletTypeIndex++;
                    }
                }
            }

            // If we haven't found a default pallet type, look for the white one way pallet type.
            if (!foundDefaultPalletType)
            {
                if (cboPalletType.Items.Count == 1)
                    cboPalletType.Items[0].Selected = true;
                else
                {
                    defaultPalletTypeIndex = 0;
                    RadComboBoxItem li = cboPalletType.FindItemByText("white one way", true);
                    if (li != null)
                        li.Selected = true;

                }

                //defaultPalletTypeIndex = 0;
                //// None of the rows can be marked as "IsActive" thus there is no filter set.
                //// Loop round each pallet type and look for the "White one way" pallet type.
                //foreach (DataRow row in vwActivePalletTypes.Table.Rows)
                //{
                //    if (row["Description"].ToString().ToLower().Trim() == "white one way")
                //    {
                //        cboPalletType.Items[defaultPalletTypeIndex].Selected = true;
                //    }
                //    else
                //    {
                //        defaultPalletTypeIndex++;
                //    }
                //}
            }
        }

        private void SetDefaultCollectionBookingType()
        {
            switch (Globals.Configuration.DefaultCollectionBookingType)
            {
                case 0:
                    {
                        rdCollectionBookingWindow.Checked = false;
                        rdCollectionIsAnytime.Checked = false;
                        rdCollectionTimedBooking.Checked = true;

                        hidCollectionTimingMethod.Value = "timed";
                        dteCollectionFromTime.Enabled = true;
                        break;
                    }
                case 1:
                    {
                        rdCollectionBookingWindow.Checked = true;
                        rdCollectionIsAnytime.Checked = false;
                        rdCollectionTimedBooking.Checked = false;

                        hidCollectionTimingMethod.Value = "window";
                        dteCollectionFromTime.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        rdCollectionBookingWindow.Checked = false;
                        rdCollectionIsAnytime.Checked = true;
                        rdCollectionTimedBooking.Checked = false;

                        hidCollectionTimingMethod.Value = "anytime";
                        dteCollectionFromTime.Enabled = false;
                        break;
                    }
            }
        }

        private void SetDefaultDeliveryBookingType()
        {
            switch (Globals.Configuration.DefaultDeliveryBookingType)
            {
                case 0:
                    {
                        rdDeliveryBookingWindow.Checked = false;
                        rdDeliveryIsAnytime.Checked = false;
                        rdDeliveryTimedBooking.Checked = true;

                        hidDeliveryTimingMethod.Value = "timed";
                        dteDeliveryByTime.Enabled = true;
                        break;
                    }
                case 1:
                    {
                        rdDeliveryBookingWindow.Checked = true;
                        rdDeliveryIsAnytime.Checked = false;
                        rdDeliveryTimedBooking.Checked = false;

                        hidDeliveryTimingMethod.Value = "window";
                        dteDeliveryByTime.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        rdDeliveryBookingWindow.Checked = false;
                        rdDeliveryIsAnytime.Checked = true;
                        rdDeliveryTimedBooking.Checked = false;

                        hidDeliveryTimingMethod.Value = "anytime";
                        dteDeliveryByTime.Enabled = false;
                        break;
                    }
            }
        }

        private void LoadServiceLevels()
        {
            Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
            DataSet dsServices = facOrder.GetActiveForOrganisation(ClientIdentityID);

            cboService.DataSource = dsServices;
            cboService.DataTextField = "Description";
            cboService.DataValueField = "OrderServiceLevelID";
            cboService.DataBind();

            lvServiceLevelDays.DataSource = dsServices;
            lvServiceLevelDays.DataBind();

            RadComboBoxItem li = null;

            // Set the default Service Level
            if (Orchestrator.Globals.Configuration.DefaultOrderServiceLevel != string.Empty)
                li = cboService.FindItemByText(Orchestrator.Globals.Configuration.DefaultOrderServiceLevel);
            else
                li = cboService.FindItemByValue("1");

            if (li != null)
                li.Selected = true;
        }

        private void LoadGoodsTypes()
        {
            DataSet dsGoodsTypes = Orchestrator.Facade.GoodsType.GetAllActiveGoodsTypes();
            cboGoodsType.DataSource = dsGoodsTypes;
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataValueField = "GoodsTypeID";
            cboGoodsType.DataBind();

            cboGoodsType.Items[0].Selected = true;
        }

        #endregion

        #region Utilities

        private void SetFocus(string clientID)
        {
            if (!string.IsNullOrEmpty(clientID))
                litSetFocus.Text = "<script type=\"text/javascript\" language=\"javascript\">SetFocus('" + clientID + "');</script>";
            else
                litSetFocus.Text = string.Empty;
        }

        #endregion

        #endregion

        #region Event Handlers

        #region Buttons

        private void ShowConfirmationWindow()
        {
            string openOrderGroupProfileJS = dlgOrderConfirmation.GetOpenDialogScript(string.Format("OID={0}", OrderID));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", openOrderGroupProfileJS, true);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.FacadeResult retVal = new Entities.FacadeResult();
                
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                Entities.Order order = facOrder.GetForOrderID(this.OrderID);

                IEnumerable<Entities.Extra> surcharges = null;
                PopulateOrder(order, out surcharges);

                retVal.Success = facOrder.Update(order, this.Page.User.Identity.Name);

                OrderID = order.OrderID;
                if (retVal.Success)
                {
                    lblInformation.Text = "The order has been updated. <b>Order ID :: " + order.OrderID.ToString() + "</b>";
                    lblInformation.Visible = true;
                    divInformation.Visible = true;
                    lblInformation.ForeColor = System.Drawing.Color.Black;

                    ucInfringments.Infringements = null;
                    ucInfringments.DisplayInfringments();
                }
                else
                {
                    ucInfringments.Infringements = retVal.Infringements;
                    ucInfringments.DisplayInfringments();

                    lblInformation.Text = "The order has not been updated. <b>Order ID :: " + order.OrderID.ToString() + "</b>";
                    lblInformation.Visible = true;
                    divInformation.Visible = true;
                    lblInformation.ForeColor = System.Drawing.Color.Red;
                }

                btnBack.Visible = true;

            }
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            this.AddOrder();
        }

        void btnAddAndReset_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            this.AddOrder();

            // Reset the page
            this.OrderID = 0;
            this.ClearScreen();
            this.ShowOrHidePalletNetworkFields();
            this.PageLoadNewOrder();
        }

        private void AddOrder()
        {
            // Populate the order, extras and vigo order
            var order = new Entities.Order();
            IEnumerable<Entities.Extra> extras;
            this.PopulateOrder(order, out extras);
            EF.VigoOrder vigoOrder = this.BuildVigoOrder(order, extras);

            // Create the order
            Facade.IOrder facOrder = new Facade.Order();
            var userName = ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;
            this.OrderID = facOrder.Create(order, extras, userName);
            order.OrderID = this.OrderID;
            lblInformation.Text = "The order has been added. <b>Order ID :: " + OrderID.ToString() + "</b>";

            if (order.OrderID > 0)
            {
                // Create the vigo order
                if (vigoOrder != null)
                {
                    if (vigoOrder.Order == null)
                    {
                        vigoOrder.OrderId = OrderID;
                        vigoOrder.OrderReference.EntityKey = EF.DataContext.CreateKey("OrderSet", "OrderId", OrderID);
                    }

                    EF.DataContext.Current.SaveChanges();
                }

                this.ShowConfirmationWindow();
            }
        }

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (OrderID != -1)
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

        void btnCancel_ServerClick(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            bool result = facOrder.Cancel(this.SavedOrder, "Cancelled by client user prior to being approved.", this.Page.User.Identity.Name);
            if (result)
            {
                lblInformation.Text = "The order has been cancelled. <b>Order ID :: " + this.SavedOrder.OrderID.ToString() + "</b>";
                lblInformation.Visible = true;
                divInformation.Visible = true;
                lblInformation.ForeColor = System.Drawing.Color.Black;
                divOuter.Disabled = true;
                this.lblCancelledIndicator1.Visible = true;
                this.lblCancelledIndicator2.Visible = true;
            }
            else
            {
                lblInformation.Text = "Is was not possible to cancel the order.";
                lblInformation.Visible = true;
                divInformation.Visible = true;
                lblInformation.ForeColor = System.Drawing.Color.Red;
                this.lblCancelledIndicator1.Visible = false;
                this.lblCancelledIndicator2.Visible = false;
            }
        }

        #endregion

        #region Link

        void lnkReset_Click(object sender, EventArgs e)
        {
            // Clear all of the fields.
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        #endregion

        #region Repeaters

        private void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Orchestrator.Entities.OrganisationReferenceCollection organisationReferences = (Orchestrator.Entities.OrganisationReferenceCollection)repReferences.DataSource;

                HtmlInputHidden hidOrganisationReferenceId = (HtmlInputHidden)e.Item.FindControl("hidOrganisationReferenceId");
                PlaceHolder plcHolder = (PlaceHolder)e.Item.FindControl("plcHolder");

                int organisationReferenceId = Convert.ToInt32(hidOrganisationReferenceId.Value);

                if (_isUpdate)
                {
                    // Make sure the value is in place
                    TextBox txtReferenceValue = (TextBox)e.Item.FindControl("txtReferenceValue");
                    Orchestrator.Entities.OrderReference currentValue = this.SavedOrder.GetForOrganisationReferenceID(organisationReferenceId);
                    if (currentValue != null)
                        txtReferenceValue.Text = currentValue.Reference;
                }

                // Configure the validator controls
                CustomValidator validatorControl = null;

                Orchestrator.Entities.OrganisationReference reference = organisationReferences.FindByReferenceId(organisationReferenceId);
                switch (reference.DataType)
                {
                    case eOrganisationReferenceDataType.Decimal:
                        validatorControl = new CustomValidator();
                        validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateDecimal);
                        break;
                    case eOrganisationReferenceDataType.FreeText:
                        // No additional validation required.
                        break;
                    case eOrganisationReferenceDataType.Integer:
                        validatorControl = new CustomValidator();
                        validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateInteger);
                        break;
                }

                if (validatorControl != null)
                {
                    // Configure the validator properties
                    validatorControl.ControlToValidate = "txtReferenceValue";
                    validatorControl.Display = ValidatorDisplay.Dynamic;
                    validatorControl.ErrorMessage = "Please supply a " + reference.Description + ".";
                    validatorControl.Text = "<img src=\"../../images/error.png\"  Title=\"Please supply a " + reference.Description + ".\" />";
                    validatorControl.EnableClientScript = false;

                    plcHolder.Controls.Add(validatorControl);
                }
            }
        }

        #endregion

        #endregion

        #region Private Functions

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

                    this.ucCollectionPoint.ClientUserOrganisationIdentityID = ClientIdentityID;
                    this.ucDeliveryPoint.ClientUserOrganisationIdentityID = ClientIdentityID;
                }
                else
                {
                    throw new ApplicationException("User is not a client user.");
                }
            }
        }

        #region Client Reference Details

        private void ConfigureClientReferences()
        {
            if (ClientIdentityID != 0)
            {
                Orchestrator.Facade.IOrganisationReference facOrganisationReference = new Orchestrator.Facade.Organisation();
                _clientReferences = facOrganisationReference.GetReferencesForOrganisationIdentityId(ClientIdentityID, true);

                if (_clientReferences != null)
                {
                    // Populate the repeater control with the required reference fields
                    repReferences.DataSource = _clientReferences;
                    repReferences.DataBind();
                }

            }
        }

        #endregion

        #region Set Default Collection Point

        private void SetDefaultCollectionPoint()
        {
            try
            {
                Facade.Organisation facOrganisation = new Orchestrator.Facade.Organisation();
                Entities.Organisation client = facOrganisation.GetForIdentityId(ClientIdentityID);

                if (client.Defaults[0].DefaultCollectionPointId > 0)
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    Entities.Point defaultPoint = facPoint.GetPointForPointId(client.Defaults[0].DefaultCollectionPointId);
                    ucCollectionPoint.SelectedPoint = defaultPoint;
                }
                else
                {
                    ucCollectionPoint.SelectedPoint = null;
                }
            }
            catch
            {
                // TODO: Remove this empty catch statement.
            }
        }

        #endregion

        #region Entity Handling

        /// <summary>
        /// Populate the order based on the entered data
        /// </summary>
        /// <param name="order">A new or existing order</param>
        /// <param name="extras">The generated list of extras - note this is only returned for new orders, it will be null for updates since clients can't update extras on an order.</param>
        private void PopulateOrder(Entities.Order order, out IEnumerable<Entities.Extra> extras)
        {
            // Make sure the correct rate information is used.
            CheckForQuickSubmission();

            extras = new List<Entities.Extra>();

            if (order.OrderID == 0)
                order.OrderInstructionID = OrderInstructionID;

            var collectionPointID = ucCollectionPoint.PointID;
            var collectionDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionFromDate, dteCollectionFromTime);
            var collectionByDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionByDate, dteCollectionByTime);

            var deliveryPointID = ucDeliveryPoint.PointID;
            var deliveryFromDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryFromDate, dteDeliveryFromTime);
            var deliveryDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryByDate, dteDeliveryByTime);

            var palletSpaces = 0m;
            var noPallets = 0;

            if (hidShowPalletNetworkFields.Value == "true")
            {
                noPallets = 0;
                noPallets += (int)(rntxtFullPallets.Value ?? 0);
                noPallets += (int)(rntxtHalfPallets.Value ?? 0);
                noPallets += (int)(rntxtQtrPallets.Value ?? 0);
                noPallets += (int)(rntxtOverSizePallets.Value ?? 0);

                palletSpaces = noPallets;
            }
            else
            {
                noPallets = int.Parse(txtNoPallets.Text);
                palletSpaces = rntxtPalletSpaces.Value.HasValue ? (decimal)rntxtPalletSpaces.Value.Value : 0m;
            }

            var weight = 0m;
            decimal.TryParse(txtWeight.Text, out weight);

            int cases = 0;
            int.TryParse(txtCases.Text, out cases);

            var rate = rntRate.Value.HasValue ? (decimal)rntRate.Value.Value : 0m;

            var tariffRate = 0m;
            var hasTariffRate = !string.IsNullOrWhiteSpace(hidTariffRate.Value) && decimal.TryParse(hidTariffRate.Value, out tariffRate);
            var canSetTariffOverride = false;
            var isTariffRateOverridden = false;

            if (hasTariffRate && rate != tariffRate)
            {
                if (order.OrderID == 0 || rate != order.ForeignRate)
                {
                    canSetTariffOverride = true;
                    isTariffRateOverridden = true;
                }
            }
            else
            {
                if (order.OrderID == 0)
                {
                    canSetTariffOverride = true;
                    isTariffRateOverridden = false;
                }
            }

            if (order.OrderID == 0)
            {
                // New order so it is possible to add extras.
                extras = GetSelectedExtras();
            }

            var orderReferences = repReferences.Items.Cast<RepeaterItem>().ToDictionary(
                i => Convert.ToInt32(((HtmlInputHidden)i.FindControl("hidOrganisationReferenceId")).Value),
                i => ((TextBox)i.FindControl("txtReferenceValue")).Text);

            Facade.IOrder facOrder = new Facade.Order();

            facOrder.PopulateOrder(
                order,
                this.ClientIdentityID,
                this.GetDefaultBusinessTypeID(),
                txtLoadNumber.Text,
                txtDeliveryOrderNumber.Text,
                noPallets,
                weight,
                int.Parse(cboGoodsType.SelectedValue),
                txtNotes.Text,
                null,
                null,
                null,
                null,
                cases,
                int.Parse(cboService.SelectedValue),
                palletSpaces,
                int.Parse(cboPalletType.SelectedValue),
                collectionPointID,
                collectionDateTime,
                collectionByDateTime,
                deliveryPointID,
                deliveryFromDateTime,
                deliveryDateTime,
                false,
                null,
                false,
                false,
                false,
                null,
                null,
                rate,
                rntRate.Culture.LCID,
                canSetTariffOverride,
                isTariffRateOverridden,
                isTariffRateOverridden,
                false,
                null,
                true,
                0m,
                false,
                false,
                orderReferences,
                extras,
                this.IsClientUser,
                Page.User.Identity.Name);
        }

        private IEnumerable<Entities.Extra> GetSelectedExtras()
        {
            var extras = new List<Entities.Extra>();

            if (!string.IsNullOrWhiteSpace(hidSelectedSurchargeExtraTypeIDs.Value))
            {
                string[] selectedSurchargeDetails = hidSelectedSurchargeExtraTypeIDs.Value.Split('|');

                if (selectedSurchargeDetails.Any())
                {
                    foreach (string val in selectedSurchargeDetails)
                    {
                        int extraTypeID = 0;
                        decimal extraRate = decimal.Zero;
                        string[] parts = val.Split('-');

                        if (int.TryParse(parts[0], out extraTypeID) && decimal.TryParse(parts[1], out extraRate))
                        {
                            // The user has opted to create this extra against the order.
                            var extra = new Entities.Extra
                            {
                                ForeignAmount = extraRate,
                                ExtraAmount = extraRate,
                                JobId = 0,
                                OrderID = 0,
                                ExtraState = eExtraState.Accepted,
                                ExtraType = (eExtraType)extraTypeID,
                                ClientContact = "Surcharge",
                            };

                            extras.Add(extra);
                        }
                    }
                }
            }

            return extras;
        }

        private static DateTime GetSelectedCollectionOrDeliveryDateTime(RadDatePicker datePicker, RadTimePicker timePicker)
        {
            return datePicker.SelectedDate.Value.Date.Add(
                new TimeSpan(
                    timePicker.SelectedDate.Value.Hour,
                    timePicker.SelectedDate.Value.Minute,
                    0)
            );
        }

        private int GetDefaultBusinessTypeID()
        {
            int retVal;

            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.Organisation customerOrg = facOrg.GetForIdentityId(ClientIdentityID);

            if (customerOrg != null && customerOrg.Defaults.Count > 0 && customerOrg.Defaults[0].DefaultBusinessTypeID > 0)
                retVal = customerOrg.Defaults[0].DefaultBusinessTypeID;
            else
            {
                Facade.IBusinessType facBT = new Orchestrator.Facade.BusinessType();
                DataSet businessTypes = facBT.GetAll();
                retVal = (int)businessTypes.Tables[0].Rows[0]["BusinessTypeId"];
            }

            return retVal;
        }

        private string GetSurchargeDescription(Entities.Order order, EF.VigoOrder vigoOrder, List<EF.ExtraType> extraTypes)
        {
            string description = string.Empty;

            foreach (var vigoOrderExtra in vigoOrder.VigoOrderExtras)
            {
                //Some hard coded nastiness that will need to be changed at some point...

                //BW    Delivery between [deliver from time] – [deliver by time] hrs.
                //BI    Please book in.
                //AM    AM delivery.
                //TB    Deliver at [deliver by time] hrs. or if not A service: Deliver on [delivery date] at [deliver by time] hrs.
                //EB    Deliver on [delivery date].
                //TL    Tail-lift delivery.
                //SA    SATURDAY AM DELIVERY.
                //TA    Deliver by 10am.

                switch (vigoOrderExtra.ExtraType.ShortDescription.ToUpper())
                {
                    case "BW":
                        description = string.Format("Delivery between {0} – {1} hrs", order.DeliveryFromDateTime.ToString("HH:mm"), order.DeliveryDateTime.ToString("HH:mm"));
                        break;

                    case "BI":
                        description = "Please book in";
                        break;

                    case "AM":
                        description = "AM delivery";
                        break;

                    case "TB":
                        description = string.Format("Deliver on {0} at {1} hrs", order.DeliveryDateTime.ToString("dd/MM/yy"), order.DeliveryDateTime.ToString("HH:mm"));
                        break;

                    case "EB":
                        description = string.Format("Deliver on {0}", order.DeliveryDateTime.ToString("dd/MM/yy"));
                        break;

                    case "SA":
                        description = "SATURDAY AM DELIVERY";
                        break;

                    case "TA":
                        description = "Deliver by 10am";
                        break;

                    default:
                        break;
                }
            }

            return description;
        }

        EF.VigoOrder BuildVigoOrder(Entities.Order order, IEnumerable<Entities.Extra> extras)
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(order.BusinessTypeID);

            EF.VigoOrder vigoOrder = null;

            //Get the PalletForce fields if the BusinessType is PalletForce
            if (businessType.IsPalletNetwork)
            {
                vigoOrder = new EF.VigoOrder();
                EF.DataContext.Current.AddToVigoOrderSet(vigoOrder);

                Facade.IVigoOrder facVigoOrder = new Facade.Order();

                facVigoOrder.PopulateVigoOrder(
                    vigoOrder,
                    this.SavedOrder == null,
                    (int?)rntxtFullPallets.Value ?? 0,
                    (int?)rntxtHalfPallets.Value ?? 0,
                    (int?)rntxtQtrPallets.Value ?? 0,
                    (int?)rntxtOverSizePallets.Value ?? 0,
                    order.OrderServiceLevelID,
                    order.BusinessTypeID,
                    ucCollectionPoint.Depot,
                    ucDeliveryPoint.Depot,
                    ucDeliveryPoint.CountryCode,
                    order.DeliveryFromDateTime,
                    order.DeliveryDateTime);

                //Now add Extras for the selected extras remaining
                List<EF.ExtraType> extraTypes = EF.DataContext.Current.ExtraTypeSet.Select(et => et).ToList();
                foreach (var extra in extras)
                {
                    var vigoOrderExtra = new EF.VigoOrderExtra();
                    int extraTypeId = (int)extra.ExtraType;
                    vigoOrderExtra.ExtraType = extraTypes.First(et => et.ExtraTypeId == extraTypeId);
                    vigoOrder.VigoOrderExtras.Add(vigoOrderExtra);
                }
            }
            else
                vigoOrder = null;

            return vigoOrder;

        }

        /// <summary>
        /// The user may have been too quick and submitted the page before the "get rate" method returned.
        /// This method tests for this (i.e. does the rate in the hidden field match the rate we get now),
        /// need to check for rate overrides.
        /// </summary>
        private void CheckForQuickSubmission()
        {
            DateTime collectionDateTime = (DateTime)dteCollectionFromDate.SelectedDate;
            collectionDateTime = collectionDateTime.Subtract(collectionDateTime.TimeOfDay);
            if (dteCollectionFromTime.SelectedDate == null)
                collectionDateTime = collectionDateTime.Add(new TimeSpan(23, 59, 59));
            else
                collectionDateTime = collectionDateTime.Add(new TimeSpan(((DateTime)dteCollectionFromTime.SelectedDate).TimeOfDay.Hours, ((DateTime)dteCollectionFromTime.SelectedDate).TimeOfDay.Minutes, 0));

            DateTime deliveryDateTime = (DateTime)dteDeliveryByDate.SelectedDate;
            deliveryDateTime = deliveryDateTime.Subtract(deliveryDateTime.TimeOfDay);
            if (dteDeliveryByTime.SelectedDate == null)
                deliveryDateTime = deliveryDateTime.Add(new TimeSpan(23, 59, 59));
            else
                deliveryDateTime = deliveryDateTime.Add(new TimeSpan(((DateTime)dteDeliveryByTime.SelectedDate).TimeOfDay.Hours, ((DateTime)dteDeliveryByTime.SelectedDate).TimeOfDay.Minutes, 0));

            Facade.IOrder facOrder = new Facade.Order();
            IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;
            //I believe an exception gets throwwn if the postcode cannot be zoned
            Repositories.DTOs.RateInformation correctRateInformation = null;

            try
            {
                correctRateInformation = facOrder.GetRate(ClientIdentityID, BusinessTypeID, OrderInstructionID, ucCollectionPoint.PointID, ucDeliveryPoint.PointID, int.Parse(cboPalletType.SelectedValue), decimal.Parse(rntxtPalletSpaces.Text), int.Parse(txtWeight.Text), int.Parse(cboGoodsType.SelectedValue), int.Parse(cboService.SelectedValue), collectionDateTime, deliveryDateTime, true, out surcharges, true);
            }
            catch { }


            if (correctRateInformation != null && correctRateInformation.Rate.HasValue)
            {
                if (!String.IsNullOrEmpty(hidTariffRate.Value) && correctRateInformation.Rate.Value != decimal.Parse(hidTariffRate.Value)) // hidTariffRateDescription.Value)
                {

                    // A different rate was detected, the user must have submitted too quickly.
                    // Apply the new rate to the controls unless the user has overridden the rate.
                    decimal userRate = decimal.Zero;
                    decimal tariffRate = decimal.Zero;

                    userRate = (decimal)rntRate.Value.Value;
                    decimal.TryParse(hidTariffRate.Value, out tariffRate);

                    if (userRate == tariffRate)
                    {
                        // The rate was not overridden, update the specified rate.
                        rntRate.Value = (double?)correctRateInformation.ForeignRate;
                    }
                    else
                    {
                        // The rate was overridden, leave the user's rate alone.
                    }

                    // Update the tariff information so the correct data is picked up when creating the order.
                    hidTariffRateDescription.Value = correctRateInformation.TariffDescription;
                    hidTariffRate.Value = correctRateInformation.ForeignRate.ToString();

                    if (!this.IsUpdate)
                    {
                        // The user may have specified that surcharges would be included that are not offered by this tariff rate's tariff version.
                        // I think the only way this can happen is if the user has just changed the delivery date for the order and picked up a
                        // different tariff version.
                        StringBuilder sbValidSurcharges = new StringBuilder();
                        foreach (string surchargeInformation in hidSelectedSurchargeExtraTypeIDs.Value.Split('|'))
                        {
                            if (surchargeInformation.Contains('-'))
                            {
                                int extraTypeID = int.Parse(surchargeInformation.Split('-')[0]);
                                var surchargeDefinition = surcharges.FirstOrDefault(s => s.ExtraTypeID == extraTypeID);
                                if (surchargeDefinition != null)
                                {
                                    // A surcharge for the same extra type is defined for the correct rate.
                                    // Use that surcharge instead.
                                    if (sbValidSurcharges.Length > 0)
                                        sbValidSurcharges.Append("|");
                                    sbValidSurcharges.Append(string.Format("{0}-{1}", surchargeDefinition.ExtraTypeID, surchargeDefinition.Rate));
                                }
                            }
                        }
                        hidSelectedSurchargeExtraTypeIDs.Value = sbValidSurcharges.ToString();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Validation

        void cfvNoPallets_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int noPallets = -1;
            args.IsValid = int.TryParse(txtNoPallets.Text, out noPallets) && noPallets > 0 & noPallets < 999;
        }

        void cfvOrderDetailValidation_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool isValid = true;
            string pointSelectionError = string.Empty;

            if (ucCollectionPoint.SelectedPoint == null || ucDeliveryPoint.SelectedPoint == null)
            {
                isValid = false;

                if (ucCollectionPoint.SelectedPoint == null)
                    pointSelectionError = string.Format("<li>Please make sure the collection point is selected.</li>", pointSelectionError);

                if (ucDeliveryPoint.SelectedPoint == null)
                    pointSelectionError = string.Format("{0}<li>Please make sure the delivery point is selected.</li>", pointSelectionError);
            }

            if (!isValid)
                litPointExistingValidatorDisplay.Text = pointSelectionError;
            else
                litPointExistingValidatorDisplay.Text = string.Empty;

            args.IsValid = isValid;
        }

        void cfvDelivery_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (dteCollectionByTime.SelectedDate == null || dteDeliveryByTime.SelectedDate == null)
            {
                return;
            }

            if (hidCollectionTimingMethod.Value.ToLower() == "timed")
            {
                dteCollectionByDate.SelectedDate = dteCollectionFromDate.SelectedDate;
                dteCollectionByTime.SelectedDate = dteCollectionFromTime.SelectedDate;
            }
            else if (hidCollectionTimingMethod.Value.ToLower() == "anytime")
            {
                dteCollectionByDate.SelectedDate = dteCollectionFromDate.SelectedDate;
                dteCollectionByTime.SelectedDate = dteCollectionFromTime.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 0));
            }

            if (hidDeliveryTimingMethod.Value.ToLower() == "timed")
            {
                dteDeliveryFromDate.SelectedDate = dteDeliveryByDate.SelectedDate;
                dteDeliveryFromTime.SelectedDate = dteDeliveryByTime.SelectedDate;
            }
            else if (hidDeliveryTimingMethod.Value.ToLower() == "anytime")
            {
                dteDeliveryFromDate.SelectedDate = dteDeliveryByDate.SelectedDate;

                // Ensure the delivery from time is 00:00 (because the user has selected 'anytime')
                dteDeliveryFromTime.SelectedDate = dteDeliveryFromTime.SelectedDate.Value.Date.Add(new TimeSpan(0, 0, 0));

                // Ensure the delivery by time is 23:59 (because the user has selected 'anytime')
                dteDeliveryByTime.SelectedDate = dteDeliveryByTime.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 0));
            }

            bool passed = true;
            string errorMessage = string.Empty;

            DateTime collectionFromDate = dteCollectionFromDate.SelectedDate.Value;
            collectionFromDate = collectionFromDate.AddHours(dteCollectionFromTime.SelectedDate.Value.Hour);
            collectionFromDate = collectionFromDate.AddMinutes(dteCollectionFromTime.SelectedDate.Value.Minute);

            DateTime collectionByDate = dteCollectionByDate.SelectedDate.Value;
            collectionByDate = collectionByDate.AddHours(dteCollectionByTime.SelectedDate.Value.Hour);
            collectionByDate = collectionByDate.AddMinutes(dteCollectionByTime.SelectedDate.Value.Minute);

            DateTime deliveryFromDate = dteDeliveryFromDate.SelectedDate.Value;
            deliveryFromDate = deliveryFromDate.AddHours(dteDeliveryFromTime.SelectedDate.Value.Hour);
            deliveryFromDate = deliveryFromDate.AddMinutes(dteDeliveryFromTime.SelectedDate.Value.Minute);

            DateTime deliveryByDate = dteDeliveryByDate.SelectedDate.Value;
            deliveryByDate = deliveryByDate.AddHours(dteDeliveryByTime.SelectedDate.Value.Hour);
            deliveryByDate = deliveryByDate.AddMinutes(dteDeliveryByTime.SelectedDate.Value.Minute);

            lblCollectionDeliveryExceptions.Text = "";

            if (hidCollectionTimingMethod.Value.ToLower() == "window")
            {
                // If booking window is selected for collections then "collection by" must occur 
                // after "collection from".
                if (!(collectionByDate > collectionFromDate))
                {
                    passed = false;
                    errorMessage = string.Format("{0}The collection by date must occur after the collection from date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = "<li>The collection by date must occur after the collection from date.";
                }

                // The collection booking window must be at least 5 mins.
                DateTime collectionFromPlus5Mins = collectionFromDate.AddMinutes(5);
                if (collectionByDate <= collectionFromPlus5Mins)
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe collection booking window must be at least 5 minutes in size.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The collection booking window must be at least 5 minutes in size.", lblCollectionDeliveryExceptions.Text);
                }
            }

            if (hidDeliveryTimingMethod.Value.ToLower() == "window")
            {
                // If booking window is selected for deliveries then "delivery by" must occur 
                // after "delivery from".
                if (!(deliveryByDate > deliveryFromDate))
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery by date must occur after the delivery from date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery by date must occur after the delivery from date.", lblCollectionDeliveryExceptions.Text);
                }

                // The delivery booking window must be at least 5 mins.
                DateTime deliveryFromPlus5Mins = deliveryFromDate.AddMinutes(5);
                if (deliveryByDate <= deliveryFromPlus5Mins)
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery booking window must be at least 5 minutes in size.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery booking window must be at least 5 minutes in size.", lblCollectionDeliveryExceptions.Text);
                }
            }


            // If timed booking is selected for collections then "collection by" must equal
            // "collection from".
            if (hidCollectionTimingMethod.Value.ToLower() == "timed")
            {
                if (!(collectionByDate == collectionFromDate))
                {
                    passed = false;
                }
            }

            // If timed booking is selected for deliveries then "delivery by" must equal
            // "delivery from".
            if (hidDeliveryTimingMethod.Value.ToLower() == "timed")
            {
                if (!(deliveryByDate == deliveryFromDate))
                {
                    passed = false;
                }
            }

            // "Delivery from" must occur after "collection by" regardless of timing method (i.e. timed booking or booking window)

            if (hidDeliveryTimingMethod.Value.ToLower() == "anytime" && hidCollectionTimingMethod.Value.ToLower() == "anytime")
            {
                if (collectionByDate.Date > deliveryFromDate.Date) /* Note: this comparison does not take the time portion into account as we're comparing anytime values. */
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery must occur after the collection date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery must occur after the collection date.", lblCollectionDeliveryExceptions.Text);
                }
            }
            else
            {
                if (collectionByDate > deliveryFromDate) /* Note: this comparison takes account of the time */
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery must occur after the collection date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery must occur after the collection date.", lblCollectionDeliveryExceptions.Text);
                }
            }

            // Warning: dates occur far in the future 
            // Has validation passed or not
            if (!passed)
            {
                imgCfvDeliveryWarning.Attributes.Add("title", errorMessage);
                args.IsValid = false;
            }
            else
                args.IsValid = true;

        }

        private void validatorControl_ServerValidateDecimal(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
        }

        private void validatorControl_ServerValidateInteger(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        #endregion

    }

}