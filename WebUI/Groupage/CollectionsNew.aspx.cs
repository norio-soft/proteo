using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Orchestrator.Entities;
using Orchestrator.WebUI;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Groupage
{

    //------------------------------------------------------------------------------

    public partial class CollectionsNew : Orchestrator.Base.BasePage
    {
        //------------------------------------------------------------------------------

        #region Private fields

        //------------------------------------------------------------------------------
        
        private enum eOrderGroupPosition { Single, First, Mid, Last };

        //------------------------------------------------------------------------------

        private bool _exportOrderEnabled = false;
        private const string C_StartDate_VS = "C_StartDate_VS";
        private const string C_EndDate_VS = "C_EndDate_VS";
        private readonly string VS_ROWFILTER = "_rowFilter";
        private const string C_GRID_NAME = "__collectionsGrid";
        private string _orders = string.Empty;
        private List<int> _ordersList;
        private List<int> _jobIDs = null;
        private const string C_ONLYSHOWNONCOLLECTED = "_collectionsOnlyShowNonCollected";
        private const string C_SELECTED_SERVICE_LEVELS = "SelectedServiceLevels";
        private const string C_SELECTED_SURCHARGES = "SelectedSurcharges";
        private int _noPallets = 0;
        private decimal _noPalletSpaces = 0;
        private decimal _weight = 0.0M;
        private Entities.Point _defaultCrossDockPoint = new Entities.Point();
        private const eDateFilterType C_DEFAULT_DATE_FILTERING = eDateFilterType.Delivery_Date;
        private const string C_SELECTED_BOOKIN_REQUIRED = "SelectedBookInRequired";
        private const string C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN = "SelectedDoesNotRequireBookingIn";
        private const string C_SELECTED_BOOKEDIN = "SelectedBookedIn";
        private const string C_SELECTED_DATE_FILTERING = "SelectedDateFiltering";
        private const string C_SELECTED_SHOW_EMPTY_TRUNK_ROWS = "SelectedShowEmptyTrunkRows";

        // -----------------------------------------------------------------------------------------------
        private const string INCLUDE_X_DOCKED_AT_OTHER_COMPANY = "IncludeXDockedAtAnotherCompany";
        private const string INCLUDE_X_DOCKED_AT_OWN_COMPANY = "IncludeXDockedAtMyCompany";
        private const string INCLUDE_NOT_PLANNED_FOR_COLLECTION = "IncludeNotPlannedForCollection";

        // -----------------------------------------------------------------------------------------------

        private const string C_SELECTED_START_GRID_COLLAPSED = "SelectedStartGridCollapsed";
        private bool _updateGridSettings = false;

        //------------------------------------------------------------------------------

        #endregion

        #region Page Properties
        private string RowFilter
        {
            get { return (string)this.ViewState[VS_ROWFILTER]; }
            set { this.ViewState[VS_ROWFILTER] = value; }
        }

        //------------------------------------------------------------------------------

        public Entities.Point GroupageCollectionRunDeliveryPoint
        {
            get
            {
                if (_defaultCrossDockPoint == null || String.IsNullOrEmpty(_defaultCrossDockPoint.Description))
                {
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    Entities.Organisation org = facOrg.GetForIdentityId(Globals.Configuration.IdentityId);
                    Facade.IPoint facPoint = new Facade.Point();

                    if (org.Defaults[0].GroupageCollectionRunDeliveryPoint > 0)
                        this._defaultCrossDockPoint = facPoint.GetPointForPointId(org.Defaults[0].GroupageCollectionRunDeliveryPoint);
                    else
                        this._defaultCrossDockPoint = new Orchestrator.Entities.Point();
                }

                return _defaultCrossDockPoint;
            }
            set
            {
                _defaultCrossDockPoint = value;
            }
        }

        //------------------------------------------------------------------------------

        protected decimal Weight
        {
            get
            {
                _weight = 0;

                foreach (GridDataItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        _weight += (decimal)row.GetDataKeyValue("Weight");
                    }
                }

                return _weight;
            }
            set { }

        }

        //------------------------------------------------------------------------------

        protected int NoPallets
        {
            get
            {
                _noPallets = 0;

                foreach (GridDataItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        _noPallets += (int)row.GetDataKeyValue("NoPallets");
                    }
                }

                return _noPallets;
            }
            set { }

        }

        //------------------------------------------------------------------------------

        protected decimal NoPalletSpaces
        {
            get
            {
                _noPalletSpaces = 0;

                foreach (GridDataItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        _noPalletSpaces += (decimal)row.GetDataKeyValue("PalletSpaces");
                    }
                }

                return _noPalletSpaces;
            }
            set { }

        }

        //------------------------------------------------------------------------------

        public List<int> OrdersList
        {
            get
            {

                if (_ordersList == null)
                    _ordersList = new List<int>();

                _ordersList.Clear();
                int orderID = 0;
                foreach (GridItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        orderID = int.Parse(chk.Attributes["OrderID"].ToString());
                        _ordersList.Add(orderID);
                    }
                }

                return _ordersList;
            }
        }

        //------------------------------------------------------------------------------

        public string Orders
        {
            get
            {
                _orders = string.Empty;
                int orderID = 0;
                foreach (GridItem row in grdOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        orderID = int.Parse(chk.Attributes["OrderID"].ToString());
                        if (_orders.Length > 0)
                            _orders += ",";
                        _orders += orderID.ToString();
                    }
                }

                return _orders;
            }
        }

        //------------------------------------------------------------------------------

        private List<int> JobIDs
        {
            get
            {
                _jobIDs = new List<int>();
                int jobID = 0;
                foreach (GridItem row in grdOrders.Items)
                {
                    HtmlInputCheckBox chkJobID = row.FindControl("chkJobID") as HtmlInputCheckBox;

                    if (chkJobID != null && chkJobID.Checked)
                    {
                        jobID = int.Parse(chkJobID.Value);
                        _jobIDs.Add(jobID);
                    }
                }
                return _jobIDs;
            }
        }

        //------------------------------------------------------------------------------
        #endregion

        //------------------------------------------------------------------------------

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            _exportOrderEnabled = bool.Parse(ConfigurationManager.AppSettings["ExportOrderEnabled"].ToLower());

            if (!IsPostBack)
            {
                //if (!string.IsNullOrEmpty(Request.QueryString["rm"]))
                //{
                //    bool excludeFirstline = bool.Parse(Request.QueryString["excludeFirstLine"]);
                //    int extraRows = int.Parse(Request.QueryString["extraRows"]);
                //    bool usePlannedTimes = bool.Parse(Request.QueryString["usePlannedTimes"]);
                //    if (!String.IsNullOrEmpty(Request.QueryString["isSubby"]))
                //        ShowSubbyManifest(int.Parse(Request.QueryString["rm"]), excludeFirstline, usePlannedTimes, extraRows);
                //    else
                //    {
                //        bool showFullAddress = (Request.QueryString["showFullAddress"] == null) ? false : bool.Parse(Request.QueryString["showFullAddress"]);
                //        GenerateAndShowManifest(int.Parse(Request.QueryString["rm"]), excludeFirstline, extraRows, usePlannedTimes, showFullAddress);
                //    }
                //}

                // Show/hide the "Export to Palletforce" button depending on whether a module id is present in the config.
                btnExportOrder.Visible = _exportOrderEnabled;

                rblDateFiltering.Items.Clear();

                // amended to remove the var so that I can deploy a hot fix 
                foreach (eDateFilterType item in Enum.GetValues(typeof(eDateFilterType)))
                    rblDateFiltering.Items.Add(new ListItem(Enum.GetName(typeof(eDateFilterType), item).Replace("_", " "), ((int)item).ToString()));

                rblDateFiltering.Items.Add(new ListItem("Trunk Date", "3"));

                ListItem defaultDateFiltering = rblDateFiltering.Items.FindByValue(((int)C_DEFAULT_DATE_FILTERING).ToString());
                if (defaultDateFiltering != null)
                    defaultDateFiltering.Selected = true;
                if (rblDateFiltering.Items.Count > 0 && rblDateFiltering.SelectedItem == null)
                    rblDateFiltering.Items[0].Selected = true;


                //TODO: Replace this on Collection and Deliveries
                //dteStartDate.MinValue = DateTime.Today;                            
                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
                cblTrafficAreas.DataSource = facTrafficArea.GetAll();
                cblTrafficAreas.DataBind();

                foreach (ListItem item in cblTrafficAreas.Items)
                {
                    item.Attributes.Add("onclick", "onTrafficAreaChecked(this);");
                }

                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                DataSet dsBusinessTypes = facBusinessType.GetAll();

                foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                {
                    RadMenuItem rmi = new RadMenuItem();
                    rmi.Text = row["Description"].ToString();
                    rmi.Value = row["BusinessTypeID"].ToString();
                    RadMenu1.Items.Add(rmi);
                }

                if (Session[INCLUDE_X_DOCKED_AT_OTHER_COMPANY] != null && Session[INCLUDE_X_DOCKED_AT_OTHER_COMPANY] is bool)
                    chkIncludeOrdersXDockedAtOtherCompany.Checked = (bool)Session[INCLUDE_X_DOCKED_AT_OTHER_COMPANY];

                if (Session[INCLUDE_X_DOCKED_AT_OWN_COMPANY] != null && Session[INCLUDE_X_DOCKED_AT_OWN_COMPANY] is bool)
                    chkIncludeOrdersXDockedAtOwnCompany.Checked = (bool)Session[INCLUDE_X_DOCKED_AT_OWN_COMPANY];

                if (Session[INCLUDE_NOT_PLANNED_FOR_COLLECTION] != null && Session[INCLUDE_NOT_PLANNED_FOR_COLLECTION] is bool)
                    chkIncludeOrdersNotPlannedForCollection.Checked = (bool)Session[INCLUDE_NOT_PLANNED_FOR_COLLECTION];

                // Get the Dates from the filter if one exists
                GetDates();

                ucDeliveryPoint.SelectedPoint = this.GroupageCollectionRunDeliveryPoint;

                // get the filter options from the cookie if it exists
                ConfigureDisplay();

                ConfigureOrganisationReferences();

                LoadGridSettings();
                StoreFilterOptions();
            }
        }

        //------------------------------------------------------------------------------

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (_updateGridSettings)
                SaveGridSettings(false);
        }

        //------------------------------------------------------------------------------

        #endregion

        private void ConfigureDisplay()
        {
            // Save any changes that may have been made before continuing
            //StoreFilterOptions();

            if (Request.Cookies["Collections"] != null)
            {
                HttpCookie collectionsCookie = Request.Cookies["Collections"];

                if (collectionsCookie[C_SELECTED_DATE_FILTERING] != null)
                    rblDateFiltering.SelectedValue = collectionsCookie[C_SELECTED_DATE_FILTERING];
                if (collectionsCookie[INCLUDE_NOT_PLANNED_FOR_COLLECTION] != null)
                    chkIncludeOrdersNotPlannedForCollection.Checked = bool.Parse(collectionsCookie[INCLUDE_NOT_PLANNED_FOR_COLLECTION]);
                if (collectionsCookie[INCLUDE_X_DOCKED_AT_OWN_COMPANY] != null)
                    chkIncludeOrdersXDockedAtOwnCompany.Checked = bool.Parse(collectionsCookie[INCLUDE_X_DOCKED_AT_OWN_COMPANY]);
                if (collectionsCookie[INCLUDE_X_DOCKED_AT_OTHER_COMPANY] != null)
                    chkIncludeOrdersXDockedAtOtherCompany.Checked = bool.Parse(collectionsCookie[INCLUDE_X_DOCKED_AT_OTHER_COMPANY]);
                if (collectionsCookie[C_SELECTED_BOOKIN_REQUIRED] != null)
                    chkRequiresBookingIn.Checked = bool.Parse(collectionsCookie[C_SELECTED_BOOKIN_REQUIRED]);
                if (collectionsCookie[C_SELECTED_BOOKEDIN] != null)
                    chkBookedIn.Checked = bool.Parse(collectionsCookie[C_SELECTED_BOOKEDIN]);
                if (collectionsCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN] != null)
                    chkDoesNotRequireBookingIn.Checked = bool.Parse(collectionsCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN]);
                if (collectionsCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS] != null)
                    chkNoTrunkDate.Checked = bool.Parse(collectionsCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS]);
                if (collectionsCookie[C_SELECTED_START_GRID_COLLAPSED] != null)
                    chkStartCollapsed.Checked = bool.Parse(collectionsCookie[C_SELECTED_START_GRID_COLLAPSED]);
            }
        }

        //------------------------------------------------------------------------------

        private void StoreFilterOptions()
        {

            //TDOD: Make this a permanent storage 
            // Store the selected surcharge and service level ids.
            HttpCookie collectionsCookie = new HttpCookie("collections");
            collectionsCookie.Expires = DateTime.Today.AddYears(10);
            collectionsCookie[C_SELECTED_BOOKEDIN] = this.chkBookedIn.Checked.ToString();
            collectionsCookie[C_SELECTED_BOOKIN_REQUIRED] = this.chkRequiresBookingIn.Checked.ToString();
            collectionsCookie[C_SELECTED_DATE_FILTERING] = this.rblDateFiltering.SelectedValue;
            collectionsCookie[C_SELECTED_START_GRID_COLLAPSED] = this.chkStartCollapsed.Checked.ToString();
            collectionsCookie[INCLUDE_NOT_PLANNED_FOR_COLLECTION] = this.chkIncludeOrdersNotPlannedForCollection.Checked.ToString();
            collectionsCookie[INCLUDE_X_DOCKED_AT_OTHER_COMPANY] = this.chkIncludeOrdersXDockedAtOtherCompany.Checked.ToString();
            collectionsCookie[INCLUDE_X_DOCKED_AT_OWN_COMPANY] = this.chkIncludeOrdersXDockedAtOwnCompany.Checked.ToString();
            collectionsCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN] = this.chkDoesNotRequireBookingIn.Checked.ToString();
            collectionsCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS] = this.chkNoTrunkDate.Checked.ToString();

            this.Response.Cookies.Add(collectionsCookie);
        }

        //------------------------------------------------------------------------------

        Entities.TrafficSheetFilter GetFilter()
        {
            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (_filter == null)
                _filter = GetDefaultFilter();
            return _filter;
        }

        //------------------------------------------------------------------------------

        private Entities.TrafficSheetFilter GetDefaultFilter()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            Entities.TrafficSheetFilter ts = facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

            return ts;
        }

        //------------------------------------------------------------------------------

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }

        //------------------------------------------------------------------------------

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        //------------------------------------------------------------------------------

        private void GetDates()
        {
            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (_filter != null)
            {
                if (_filter.FilterStartDate == DateTime.MinValue)
                {
                    dteStartDate.SelectedDate = DateTime.Today.Subtract(DateTime.Now.TimeOfDay);
                    dteEndDate.SelectedDate = dteStartDate.SelectedDate.Value.Add(new TimeSpan(23, 59, 59));
                }
                else
                {
                    dteStartDate.SelectedDate = _filter.FilterStartDate;
                    dteEndDate.SelectedDate = _filter.FilterEnddate;
                }

                cblTrafficAreas.ClearSelection();

                foreach (ListItem li in cblTrafficAreas.Items)
                {
                    if (_filter.TrafficAreaIDs.Exists(i => i.ToString().Equals(li.Value)))
                        li.Selected = true;
                }

                businessTypeCheckList.SelectedBusinessTypeIDs = _filter.BusinessTypes;

                rblDateFiltering.Items.FindByValue(((int)_filter.CollectionDateFilter).ToString()).Selected = true;
            }
            else
            {
                _filter = GetDefaultFilter();
                if (_filter.FilterStartDate == DateTime.MinValue)
                {
                    _filter.FilterStartDate = DateTime.Today;
                    _filter.FilterEnddate = _filter.FilterStartDate.Add(new TimeSpan(23, 59, 59));
                }
                
                dteStartDate.SelectedDate = _filter.FilterStartDate;
                dteEndDate.SelectedDate = _filter.FilterEnddate; ;

                cblTrafficAreas.ClearSelection();
                bool allSelected = true;

                foreach (ListItem li in cblTrafficAreas.Items)
                {
                    if (_filter.TrafficAreaIDs.Exists(i => i.ToString().Equals(li.Value)))
                        li.Selected = true;
                    else
                        allSelected = false;
                }

                chkSelectAllTrafficAreas.Checked = allSelected;

                var businessTypeIDs = _filter.BusinessTypes;
                var qsBusinessTypeID = Utilities.ParseNullable<int>(Request.QueryString["BT"]);

                if (qsBusinessTypeID.HasValue && !businessTypeIDs.Contains(qsBusinessTypeID.Value))
                    businessTypeIDs.Add(qsBusinessTypeID.Value);

                this.businessTypeCheckList.SelectedBusinessTypeIDs = businessTypeIDs;
            }
        }

        //------------------------------------------------------------------------------

        #region Grid Saving

        void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            SaveGridSettings(true);
            this.btnSaveGridSettings.Text = "Update Grid Layout";
        }

        //------------------------------------------------------------------------------

        private void SaveGridSettings(bool refreshGrid)
        {
            // get any columns that need to be hidden
            foreach (ListItem li in this.cblGridColumns.Items)
            {
                this.grdOrders.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            foreach (ListItem li in this.cblOrganisationReferenceColumns.Items)
            {
                this.grdOrders.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            Utilities.SaveGridSettings(this.grdOrders, eGrid.CollectionsNew, Page.User.Identity.Name);

            _updateGridSettings = false;

            // forces the grid to pick up the column settings if changed
            if (refreshGrid)
            {
                LoadGridSettings();
                grdOrders.Rebind();
            }
        }

        //------------------------------------------------------------------------------
        private void LoadGridSettings()
        {
            IEnumerable<string> columnsToHide;
            Utilities.LoadSettings(this.grdOrders, eGrid.CollectionsNew, out columnsToHide, Page.User.Identity.Name);

            foreach (ListItem li in this.cblGridColumns.Items)
            {
                li.Selected = !columnsToHide.Any(s => s == li.Value);
            }

            foreach (ListItem li in this.cblOrganisationReferenceColumns.Items)
            {
                li.Selected = !columnsToHide.Any(s => s == li.Value);
            }
        }

        #endregion Grid Saving

        //------------------------------------------------------------------------------

        #region Private Methods

        private void ConfigureOrganisationReferences()
        {
            // Add any custom organisation reference fields
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            var organisationReferences = facOrganisation.GetAllOrganisationReferences().Tables[0].AsEnumerable();

            if (organisationReferences.Any())
            {
                var groupedReferences = organisationReferences.GroupBy(or => or.Field<string>("Description"));

                foreach (var gr in groupedReferences)
                {
                    var columnDescription = gr.Key;
                    var uniqueName = OrganisationReferenceColumnUniqueName(columnDescription);

                    // Add a check box to allow the user to toggle visibility of this column
                    var listItem = new ListItem(columnDescription, uniqueName);
                    listItem.Attributes["title"] = gr.Count() > 10 ? string.Format("{0} clients", gr.Count()) : Entities.Utilities.MergeStrings("\n", gr.Select(or => or.Field<string>("OrganisationName")));
                    this.cblOrganisationReferenceColumns.Items.Add(listItem);

                    // Add the column to the grid
                    var column = new GridTemplateColumn();
                    this.grdOrders.MasterTableView.Columns.Add(column);
                    column.UniqueName = uniqueName;
                    column.HeaderText = columnDescription;
                    column.Visible = false;
                }
            }
        }

        private void CaptureOrderIDs()
        {
            List<int> orders = new List<int>();
            foreach (GridDataItem gdi in this.grdOrders.Items)
            {
                CheckBox chkOrderID = (CheckBox)gdi.FindControl("chkOrderID");
                if (chkOrderID != null && chkOrderID.Checked)
                {
                    int orderID = 0;
                    int.TryParse(chkOrderID.Attributes["OrderID"].ToString(), out orderID);

                    if (orderID > 0)
                        orders.Add(orderID);
                }
            }
            Session[C_GRID_NAME] = orders;
        }

        private string ConvertApostrophesToASCII(string originalString)
        {
            return originalString.Replace("'", "&#39"); // This is the ASCII Numerical Code for the ' symbol.
        }

        private static string OrganisationReferenceColumnUniqueName(string columnDescription)
        {
            return string.Concat("_OrgRef_", columnDescription.Replace(' ', '_'));
        }

        #endregion Private Methods

        //------------------------------------------------------------------------------

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        //------------------------------------------------------------------------------

        #region Event Handlers

        //------------------------------------------------------------------------------

        #region Button Events

        //------------------------------------------------------------------------------

        void btnChangeColumns_Click(object sender, EventArgs e)
        {
            SaveGridSettings(true);
        }

        //------------------------------------------------------------------------------

        void btnLoadingSummarySheet_Click(object sender, EventArgs e)
        {
            Facade.IJob facJob = new Facade.Job();
            List<int> LocalJobIDs = JobIDs;

            if (LocalJobIDs.Count > 0)
            {
                #region Pop-up Report
                NameValueCollection reportParams = new NameValueCollection();
                DataSet dsLSS = facJob.GetLoadingSummarySheet(LocalJobIDs);

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.LoadingSummarySheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsLSS;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");

                #endregion
            }
        }

        //------------------------------------------------------------------------------

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            //CaptureOrderIDs();
            Entities.TrafficSheetFilter _filter = GetFilter();
            _filter.FilterStartDate = dteStartDate.SelectedDate.Value;
            _filter.FilterEnddate = dteEndDate.SelectedDate.Value;
            if (_filter.FilterEnddate.TimeOfDay != new TimeSpan(23, 59, 59))
                _filter.FilterEnddate = _filter.FilterEnddate.Subtract(_filter.FilterEnddate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

            _filter.BusinessTypes.Clear();
            _filter.BusinessTypes.AddRange(businessTypeCheckList.SelectedBusinessTypeIDs);

            _filter.TrafficAreaIDs.Clear();

            foreach (ListItem li in cblTrafficAreas.Items)
            {
                if (li.Selected)
                    _filter.TrafficAreaIDs.Add(int.Parse(li.Value));
            }

            _filter.CollectionDateFilter = ((eDateFilterType)int.Parse(rblDateFiltering.SelectedValue));

            SetCookie(_filter);

            StoreFilterOptions();

            LoadGridSettings();

            grdOrders.Rebind();
        }

        public void btnRefresh_Click_NoFilterUpdate(object sender, EventArgs e)
        {
            grdOrders.Rebind();
        }

        //------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.DataBound += new EventHandler(grdOrders_DataBound);
            this.grdOrders.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.btnLoadingSummarySheet.Click += new EventHandler(btnLoadingSummarySheet_Click);
            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnSaveGridSettings.Click += new EventHandler(btnSaveGridSettings_Click);
            this.btnChangeColumns.Click += new EventHandler(btnChangeColumns_Click);

        }

        //------------------------------------------------------------------------------

        private void PreSelectItems(CheckBoxList cbl, IList items)
        {
            foreach (var id in items)
            {
                ListItem item = cbl.Items.FindByValue(id.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }

        //------------------------------------------------------------------------------

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;
            string orderIds = String.Empty;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            orderIds = this.Orders;

            if (orderIds != string.Empty)
            {
                dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(orderIds);

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

        //------------------------------------------------------------------------------

        protected void btnExportOrder_Click(object sender, EventArgs e)
        {
            // The following list will contain the ids of ALL the checked orders.
            List<int> orderIds = this.OrdersList;

            // The following list will only contain ids for the checked orders that
            // have been exported
            List<int> exportedOrderIds = null;

            if (orderIds.Count > 0)
            {
                Facade.ExportOrder facExportOrder = new Facade.ExportOrder();
                exportedOrderIds = facExportOrder.Create(orderIds, this.Page.User.Identity.Name);

                // Update the exported rows by changing their colours
                foreach (GridDataItem item in grdOrders.Items)
                {
                    CheckBox chkOrderID = (CheckBox)item.FindControl("chkOrderID");
                    int orderId;
                    int.TryParse(chkOrderID.Attributes["OrderID"].ToString(), out orderId);

                    if (exportedOrderIds.Contains(orderId))
                    {
                        item.BackColor = System.Drawing.Color.Violet;
                    }
                }

                if (orderIds.Count != exportedOrderIds.Count)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ExportPartialSuccess", "alert('It was not possible to export all of the selected orders. Note that orders that have been imported from a third party system cannot be exported.');", true);
                }
            }
            else // no orders selected
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoOrdersToExport", "alert('Please select the orders you wish to export by ticking the checkboxes on the left.');", true);
            }
        }

        //------------------------------------------------------------------------------

        #endregion Button Events

        //------------------------------------------------------------------------------

        #region Grid View Events

        //------------------------------------------------------------------------------

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                CheckBox chk = e.Item.FindControl("chkOrderID") as CheckBox;
                HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces");
                //HtmlImage imgOrderCollectionDeliveryNotes = (HtmlImage)e.Item.FindControl("imgOrderCollectionDeliveryNotes");
                HtmlImage imgOrderBookedIn = (HtmlImage)e.Item.FindControl("imgOrderBookedIn");

                List<int> orders = new List<int>();
                if (Session[C_GRID_NAME] != null)
                    orders = (List<int>)Session[C_GRID_NAME];

                DataRowView drv = (DataRowView)e.Item.DataItem;

                HtmlAnchor hypOrder = (HtmlAnchor)e.Item.FindControl("hypOrder");
                if (drv["OrderID"] != DBNull.Value)
                {
                    int orderId = (int)drv["OrderID"];
                    string queryString = string.Format("oid={0}", orderId.ToString());
                    hypOrder.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));
                    hypOrder.InnerText = orderId.ToString();
                }

                // Colour the row if the order has been exported
                if (drv["MessageStateId"] != DBNull.Value)
                {
                    eMessageState messageState = (eMessageState)drv["MessageStateId"];
                    switch (messageState)
                    {
                        case eMessageState.Unprocessed:
                            e.Item.BackColor = System.Drawing.Color.Pink;
                            break;

                        case eMessageState.Processed:
                        case eMessageState.NotProcessed:
                            e.Item.BackColor = System.Drawing.Color.Violet;
                            break;

                        case eMessageState.Error:
                            e.Item.BackColor = System.Drawing.Color.Red;
                            break;
                    }
                }

                if (chk != null)
                {
                    string collectionAddressLines = string.Empty;
                    collectionAddressLines = string.IsNullOrEmpty(drv["AddressLine1"].ToString()) ? "" : drv["AddressLine1"].ToString();
                    collectionAddressLines += string.IsNullOrEmpty(drv["AddressLine2"].ToString()) ? "" : "<br/>" + drv["AddressLine2"].ToString();
                    collectionAddressLines += string.IsNullOrEmpty(drv["AddressLine3"].ToString()) ? "" : "<br/>" + drv["AddressLine3"].ToString();
                    collectionAddressLines += string.IsNullOrEmpty(drv["PostTown"].ToString()) ? "" : "<br/>" + drv["PostTown"].ToString();

                    // orderID, noPallets, orderGroupID, orderGroupGroupedPlanning, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, isOnDeliveryJob, collectionCustomerName, collectionAddress, deliveryPointId
                    string showOrderDetails = "javascript:ChangeList(event, this, {0}, {1}, {2}, {3}, {4}, {5}, '{6}', '{7}', {8}, {9}, '{10}', '{11}', '{12}', '{13}','{14}');";

                    //Encode any spaces etc
                    string deliveryPoint = Server.HtmlEncode(drv["DeliveryPoint"].ToString());
                    string collectionPoint = Server.HtmlEncode(drv["CollectionPoint"].ToString());
                    string customer = Server.HtmlEncode(drv["Customer"].ToString());

                    //Converts apostrophes to ASCII numerical code, will be interpreted by the browser as '
                    deliveryPoint = ConvertApostrophesToASCII(deliveryPoint);
                    collectionPoint = ConvertApostrophesToASCII(collectionPoint);
                    customer = ConvertApostrophesToASCII(customer);
                    collectionAddressLines = ConvertApostrophesToASCII(collectionAddressLines);

                    chk.Attributes.Add("OrderGroupID", drv["OrderGroupID"].ToString());
                    chk.Attributes.Add("orderID", drv["OrderID"].ToString());
                    chk.Attributes.Add("onclick",
                                    string.Format(showOrderDetails,
                                                    drv["OrderID"], drv["NoPallets"], drv["OrderGroupID"],
                                                    drv["OrderGroupGroupedPlanning"].ToString().ToLower(), drv["PalletSpaces"],
                                                    drv["Weight"].ToString(), collectionPoint, deliveryPoint,
                                                    drv["CollectOrder"] == DBNull.Value ? "0" : drv["CollectOrder"].ToString(),
                                                    drv["BusinessTypeID"].ToString(), drv["BusinessType"].ToString().Trim(), Convert.ToBoolean(drv["IsOnDeliveryJob"]),
                                                    customer, collectionAddressLines, drv["DeliveryPointId"]));
                }

                PlaceHolder phPostCode = e.Item.FindControl("phPostCode") as PlaceHolder;
                if (Globals.Configuration.GPSRealtime)
                {
                    if (drv["Latitude"] != DBNull.Value && double.Parse(drv["Latitude"].ToString()) != 0)
                    {
                        HyperLink hl = new HyperLink();
                        hl.Text = drv["PostCode"].ToString();
                        hl.NavigateUrl = string.Format("javascript:CollectionPointAlterPosition({0});", drv["CollectionPointId"].ToString());
                        phPostCode.Controls.Add(hl);
                    }
                    else
                        phPostCode.Controls.Add(new LiteralControl(drv["PostCode"].ToString()));
                }
                else
                {
                    phPostCode.Controls.Add(new LiteralControl(drv["PostCode"].ToString()));
                }

                if (orders.Count > 0)
                    if (orders.Contains(int.Parse(chk.Attributes["OrderID"].ToString())))
                        if (chk != null)
                        {
                            chk.Checked = true;
                            e.Item.Selected = true;
                            NoPallets += (int)drv["NoPallets"];
                        }

                //if the order has been booked in show the tick
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.BookedIn)
                {
                    imgOrderBookedIn.Visible = true;
                    imgOrderBookedIn.Alt = string.Format("Booked In by {0} at {1}", (drv["BookedInByUserName"] == DBNull.Value) ? String.Empty : drv["BookedInByUserName"].ToString(), (drv["BookedInDateTime"] == DBNull.Value) ? String.Empty : ((DateTime)drv["BookedInDateTime"]).ToString("dd/MM/yy HH:mm"));
                }

                if (drv["IsHazardous"] != DBNull.Value && (int)drv["IsHazardous"] == 1)
                {
                    HtmlImage imgHazard = e.Item.FindControl("imgGoodsType") as HtmlImage;
                    imgHazard.Visible = true;
                }

                //if the order requires booking show the requires book in icon instead and hook up the method call
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.Required)
                {
                    imgOrderBookedIn.Src = "/images/star.gif";
                    imgOrderBookedIn.Attributes.Add("onclick", "BookIn(this," + drv["OrderID"].ToString() + ");");
                    imgOrderBookedIn.Alt = "Please click to mark as booked in.";
                    imgOrderBookedIn.Visible = true;
                }

                // Collect At column
                Label lblCollectAt = (Label)e.Item.FindControl("lblCollectAt");
                DateTime colFrom = Convert.ToDateTime(drv["CollectionDateTime"].ToString());
                DateTime colBy = Convert.ToDateTime(drv["CollectionByDateTime"].ToString());

                if (lblCollectAt != null)
                {
                    if (colFrom == colBy)
                    {
                        // Timed booking... only show a single date.
                        lblCollectAt.Text = GetDate(Convert.ToDateTime(drv["CollectionDateTime"].ToString()), false);
                    }
                    else
                    {
                        // If the times span from mignight to 23:59 on the same day then 
                        // it's an 'anytime' window.
                        if (colFrom.Date == colBy.Date && colFrom.Hour == 0 && colFrom.Minute == 0 && colBy.Hour == 23 && colBy.Minute == 59)
                        {
                            // It's anytime
                            lblCollectAt.Text = GetDate(Convert.ToDateTime(drv["CollectionDateTime"].ToString()), true);
                        }
                        else
                        {
                            // It's a booking window
                            lblCollectAt.Text = GetDate(Convert.ToDateTime(drv["CollectionDateTime"].ToString()), false) + " to " + GetDate(Convert.ToDateTime(drv["CollectionByDateTime"].ToString()), false);
                        }
                    }
                }

                // Surcharges columns
                //ITextControl lblSurcharge = (ITextControl)e.Item.FindControl("lblSurcharge");
                //lblSurcharge.Text = string.Empty;
                //int orderID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"];
                //if (this.SurchargeLookupData.ContainsKey(orderID))
                //    lblSurcharge.Text = this.SurchargeLookupData[orderID];

                // Limit the length of text displayed in the cells to save space and show a popup for the full text
                //GridDataItem gdi = e.Item as GridDataItem;
                //string gridText = e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][e.Item.UniqueID].ToString();
                //Unit colLength = e.Item.Width;
                //if (gridText.Length > (colLength.Value / 2))
                //{
                //    gdi[e.Item.UniqueID].Text = gridText.Remove(((int)colLength.Value / 2));
                //    gdi.ToolTip = gridText;
                //}

                // Populate any custom organisation reference fields
                var customReferencesXml = drv.Row.Field<string>("CustomReferences");
                if (!string.IsNullOrEmpty(customReferencesXml))
                {
                    var customReferences =
                        from cr in System.Xml.Linq.XElement.Parse(customReferencesXml).Descendants()
                        select new
                        {
                            Description = cr.Attribute("Description").Value,
                            Reference = cr.Attribute("Reference").Value,
                        };

                    foreach (var customReference in customReferences)
                    {
                        var uniqueName = OrganisationReferenceColumnUniqueName(customReference.Description);
                        var column = e.Item.OwnerTableView.Columns.FindByUniqueNameSafe(uniqueName);

                        if (column != null)
                            ((GridDataItem)e.Item)[column].Text = customReference.Reference;
                    }
                }
            }
            else if (e.Item is GridGroupHeaderItem)
            {
                GridGroupHeaderItem item = (GridGroupHeaderItem)e.Item;
                DataRowView groupDataRow = (DataRowView)e.Item.DataItem;
                item.DataCell.Text = string.Format("{0} ({1})", groupDataRow["TrafficAreaShortName"], groupDataRow["Count"]);
            }
        }

        //------------------------------------------------------------------------------

        void grdOrders_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "search")
            {
                Button btn = (Button)e.CommandSource;
                TextBox txt = btn.NamingContainer.FindControl("txtSearch") as TextBox;
                DropDownList cbo = btn.NamingContainer.FindControl("cboField") as DropDownList;
                if (source is RadGrid)
                {
                    string filterText = txt.Text;

                    string rowFilter = "{0} LIKE '{1}%'";
                    rowFilter = string.Format(rowFilter, cbo.SelectedValue, txt.Text);
                    this.RowFilter = rowFilter;
                    grdOrders.Rebind();

                }
            }

            if (e.CommandName.ToLower() == "showall")
            {
                this.RowFilter = string.Empty;
                grdOrders.Rebind();
            }
            if (e.CommandName.ToLower() == "sort")
                _updateGridSettings = true;
        }

        //------------------------------------------------------------------------------

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            List<int> trafficAreaIDs = new List<int>();
            foreach (ListItem li in cblTrafficAreas.Items)
                if (li.Selected)
                    trafficAreaIDs.Add(int.Parse(li.Value));

            Facade.IOrder facOrder = new Facade.Order();
            DateTime endDate = dteEndDate.SelectedDate.Value;

            var organisationReferenceFieldNames = string.Join(",", from li in this.cblOrganisationReferenceColumns.Items.Cast<ListItem>() where li.Selected select li.Text);

            DataSet orderData = facOrder.GetForCollection(dteStartDate.SelectedDate.Value, endDate, trafficAreaIDs, businessTypeCheckList.SelectedBusinessTypeIDs, new List<int>(), new List<int>(), rblDateFiltering.SelectedItem == null ? C_DEFAULT_DATE_FILTERING : (eDateFilterType)Enum.Parse(typeof(eDateFilterType), rblDateFiltering.SelectedItem.Value), chkRequiresBookingIn.Checked, chkBookedIn.Checked, chkNoTrunkDate.Checked, chkIncludeOrdersXDockedAtOtherCompany.Checked, chkIncludeOrdersXDockedAtOwnCompany.Checked, chkIncludeOrdersNotPlannedForCollection.Checked, chkShowAll.Checked, chkDoesNotRequireBookingIn.Checked, chkOrderOnlyShowRefusals.Checked, chkOffTrailer.Checked, organisationReferenceFieldNames);
            DataView dv = orderData.Tables[0].DefaultView;
            
            if (!string.IsNullOrEmpty(RowFilter))
                dv.RowFilter = this.RowFilter;

            grdOrders.DataSource = dv;
            grdOrders.MasterTableView.GroupsDefaultExpanded = !chkStartCollapsed.Checked;
        }

        private void grdOrders_DataBound(object sender, EventArgs e)
        {
            var groups =
                from i in grdOrders.Items.Cast<GridDataItem>()
                let orderGroupID = (int?)i.GetDataKeyValue("OrderGroupID")
                where orderGroupID > 0
                group i by orderGroupID.Value into g
                select g;

            var positionSkinIDs = new Dictionary<eOrderGroupPosition, string>
            {
                { eOrderGroupPosition.Single, "imgGridGroupPositionSingle" },
                { eOrderGroupPosition.First, "imgGridGroupPositionFirst" },
                { eOrderGroupPosition.Mid, "imgGridGroupPositionMid" },
                { eOrderGroupPosition.Last, "imgGridGroupPositionLast" },
            };
            Action<GridDataItem, eOrderGroupPosition> setOrderGroupImage = (item, pos) =>
                ((PlaceHolder)item.FindControl("plcGroupedOrderImage")).Controls.Add(new Image { SkinID = positionSkinIDs[pos] });

            foreach (var group in groups)
            {
                var count = group.Count();
                if (count == 1)
                    setOrderGroupImage(group.First(), eOrderGroupPosition.Single);
                else
                {
                    setOrderGroupImage(group.First(), eOrderGroupPosition.First);
                    foreach (var item in group.Skip(1).Take(count - 2))
                    {
                        setOrderGroupImage(item, eOrderGroupPosition.Mid);
                    }
                    setOrderGroupImage(group.Last(), eOrderGroupPosition.Last);
                }
            }
        }

        #endregion Grid View Events

        //------------------------------------------------------------------------------

        #endregion Event Handlers

        //------------------------------------------------------------------------------

        #region Page Methods for Job Creation and Manifest Production

        [System.Web.Services.WebMethod]
        public static string CreateCollectionJob(List<int> orderIDs, int businessTypeID, int deliveryPointID, DateTime? trunkDeliveryDate, DateTime? trunkDeliveryTime, int driverResourceID, int vehicleResourceID, int trailerResourceID, string userName, bool createManifest, string resourceName, int? subcontractorID, int? subcontractType, decimal? subcontractRate,bool showAsCommunicated, bool createOrderGroup)
        {
            string retVal = "";
            bool crossDock = false;

            #region Prepare Orders
            // build the orders csv
            string orders = string.Empty;
            foreach (int i in orderIDs)
            {
                if (orders.Length > 0)
                    orders += ",";

                orders += i.ToString();
            }

            // get the orders 
            Facade.IOrder facorder = new Facade.Order();
            DataSet OrderData = facorder.GetOrdersForList(orders, true, false);

            // Ensure that the Orders are sorted in delivery Order
            DataTable dtOrders = OrderData.Tables[0];
            dtOrders.PrimaryKey = new DataColumn[] { dtOrders.Columns["OrderID"] };
            dtOrders.Columns.Add("CollectionOrder", typeof(int));
            dtOrders.Columns.Add("DeliverToPointID", typeof(int));
            dtOrders.Columns.Add("DeliverAtDateTime", typeof(DateTime));
            dtOrders.Columns.Add("DeliverAtAnyTime", typeof(bool));

            DataRow row = null;
            for (int i = 0; i < orderIDs.Count; i++)
            {
                // we are using the ordinal position of the orderid in the list to denote the delivery order
                row = dtOrders.Select("OrderID =" + orderIDs[i])[0];
                row["CollectionOrder"] = i;
                row.AcceptChanges();
            }

            // If we are planning the deliveries before the collection and are planning to pick these up from a cross dock location
            // set the relevant information
            if (deliveryPointID > 0)
            {
                DateTime deliveryDateTime = trunkDeliveryDate.Value;
                if (trunkDeliveryTime.HasValue)
                    deliveryDateTime = deliveryDateTime.Add(trunkDeliveryTime.Value.TimeOfDay);

                crossDock = true;

                foreach (DataRow orderRow in dtOrders.Rows)
                {
                    orderRow["DeliverToPointID"] = deliveryPointID;
                    orderRow["DeliverAtDateTime"] = deliveryDateTime;
                    orderRow["DeliverAtAnyTime"] = !trunkDeliveryTime.HasValue;
                    orderRow.AcceptChanges();
                }
            }
            else
                foreach (DataRow orderRow in dtOrders.Rows)
                {
                    orderRow["DeliverToPointID"] = orderRow["DeliveryPointID"];
                    orderRow["DeliverAtDateTime"] = orderRow["DeliveryDateTime"];
                    orderRow["DeliverAtAnyTime"] = orderRow["DeliveryIsAnyTime"];
                    orderRow.AcceptChanges();
                }

            #endregion

            int jobID = -1;
            // GRD:-Bloody nasty thing to do here but as we are updating the orders for delivery run details here as well we need to roll together
            // GRD:- To be refactored to move this elsewhere

            try
            {
#if DEBUG
                using (var tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(5)))
#else
                using (var tran = new TransactionScope())
#endif
                {
                    #region Create Order Group

                    if (createOrderGroup && dtOrders.Rows.Count > 1)
                    {
                        int custId = (int)dtOrders.Rows[0]["CustomerIdentityId"];
                        List<int> addOrders = new List<int>();
                        foreach (DataRow orderRow in dtOrders.Rows)
                        {
                            if (custId != (int)orderRow["CustomerIdentityId"])
                            {
                                addOrders.Clear();
                                break;
                            }
                            addOrders.Add((int)orderRow["OrderID"]);
                        }

                        if (addOrders.Count > 1)
                        {
                            Facade.IOrderGroup facOrderGroup = new Facade.Order();
                            int orderGroupId = facOrderGroup.Create(addOrders[0], true,string.Empty,string.Empty, userName);
                            addOrders.RemoveAt(0);

                            var result = facOrderGroup.AddOrdersToGroup(orderGroupId, addOrders, userName);
                            if (!result.Success)
                            {
                                string msg = "Could not create order group due to the following infringements:";
                                foreach (BusinessRuleInfringement infringement in result.Infringements)
                                    msg += "\n" + infringement.Description;
                                throw new ApplicationException(msg);
                            }
                        }
                    }

                    #endregion

                    Facade.IJob facJob = new Facade.Job();
                    Entities.FacadeResult res = facJob.CreateJobForCollections(businessTypeID, OrderData, crossDock, userName);

                    if (!res.Success)
                    {
                        string msg = "Could not create run due to the following infringements:\n" + string.Join("\n", res.Infringements.Select(i => i.Description));
                        throw new ApplicationException(msg);
                    }

                    jobID = res.ObjectId;
                    var job = facJob.GetJob(jobID, true);

                    List<int> instructionIDs = new List<int>();
                    foreach (Entities.Instruction instruction in job.Instructions)
                    {
                        instructionIDs.Add(instruction.InstructionID);
                    }

                    retVal = jobID.ToString();

                    #region // resource the job

                    Facade.IInstruction facInstruction = new Facade.Instruction();

                    if (subcontractorID.HasValue && subcontractorID.Value > 0)
                    {
                        #region Sub contract the job
                        eSubContractorDataItem subOutChoice = eSubContractorDataItem.Job;
                        if (subcontractType.HasValue)
                        {
                            subOutChoice = (eSubContractorDataItem)subcontractType.Value;
                        }
                        Entities.JobSubContractor jobSubContractor = new JobSubContractor();

                        Facade.IOrganisation facOrg = new Facade.Organisation();
                        Entities.Organisation org = facOrg.GetForIdentityId(subcontractorID.Value);
                        Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                        jobSubContractor.JobID = jobID;
                        jobSubContractor.SubContractWholeJob = (subOutChoice == eSubContractorDataItem.Job);
                        jobSubContractor.ContractorIdentityId = subcontractorID.Value;
                        jobSubContractor.Rate = subcontractRate.Value;
                        jobSubContractor.UseSubContractorTrailer = true;
                        jobSubContractor.LCID = org.LCID;
                        jobSubContractor.ForeignRate = subcontractRate.Value;
                        CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
                        if (jobSubContractor.LCID != culture.LCID) // Default
                        {
                            Facade.IExchangeRates facER = new Facade.ExchangeRates();
                            jobSubContractor.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(jobSubContractor.LCID), job.Instructions.GetForInstructionType(eInstructionType.Load)[0].BookedDateTime);
                            jobSubContractor.Rate = facER.GetConvertedRate((int)jobSubContractor.ExchangeRateID, jobSubContractor.ForeignRate);
                        }
                        else
                            jobSubContractor.Rate = decimal.Round(jobSubContractor.ForeignRate, 4, MidpointRounding.AwayFromZero);

                        jobSubContractor.LCID = org.LCID;
                        if (subOutChoice == eSubContractorDataItem.Order)
                        {
                            res = facJobSubContractor.Create(jobID, new List<int>(), orderIDs, jobSubContractor, DateTime.Now, userName, true);
                        }
                        else
                            res = facJobSubContractor.Create(jobID, new List<int>(), new List<int>(), jobSubContractor, DateTime.Now, userName, true);

                        // Create theDriver Manifest for the job...
                        if (res.Success && createManifest)
                        {
                            Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
                            Entities.ResourceManifestJob rmj = null;

                            rm.ManifestDate = DateTime.Today;
                            rm.Description = resourceName + " - " + rm.ManifestDate.ToString("dd/MM/yy");
                            rm.SubcontractorId = subcontractorID;
                            rm.ResourceManifestJobs = new List<ResourceManifestJob>();
                            int jobOrder = 0;
                            foreach (Entities.Instruction insruction in job.Instructions)
                            {
                                rmj = new ResourceManifestJob();
                                rmj.InstructionId = insruction.InstructionID;
                                rmj.JobId = jobID;
                                rmj.JobOrder = jobOrder;
                                rm.ResourceManifestJobs.Add(rmj);
                                jobOrder++;
                            }
                            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
                            rm.ResourceManifestId = facResourceManifest.CreateResourceManifest(rm, userName);

                            retVal += "," + rm.ResourceManifestId.ToString() + ",1";
                        }

                        // Show be able to show as communicated event if we are not creating a manifest
                        if (res.Success && showAsCommunicated)
                        {
                            // mark the instructions as communicated so that the instruction shows as in progress
                            CommunicateInstructionsForSubContractor(jobID, subcontractorID.Value, userName);
                        }
                        #endregion
                    }
                    else
                    {
                        #region resource this to a driver if supplied and create a drive manifest
                        if (driverResourceID > 0)
                        {
                            res = facInstruction.PlanInstruction(instructionIDs, jobID, driverResourceID, vehicleResourceID, trailerResourceID, DateTime.Now, userName);

                            // Create theDriver Manifest for the job...
                            if (res.Success && createManifest)
                            {
                                Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
                                Entities.ResourceManifestJob rmj = null;

                                rm.ManifestDate = DateTime.Today;
                                rm.Description = resourceName + " - " + rm.ManifestDate.ToString("dd/MM/yy");
                                rm.ResourceId = driverResourceID;
                                rm.ResourceManifestJobs = new List<ResourceManifestJob>();
                                int jobOrder = 0;
                                foreach (Entities.Instruction insruction in job.Instructions)
                                {
                                    rmj = new ResourceManifestJob();
                                    rmj.InstructionId = insruction.InstructionID;
                                    rmj.JobId = jobID;
                                    rmj.JobOrder = jobOrder;
                                    rm.ResourceManifestJobs.Add(rmj);
                                    jobOrder++;
                                }
                                Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
                                rm.ResourceManifestId = facResourceManifest.CreateResourceManifest(rm, userName);

                                retVal += "," + rm.ResourceManifestId.ToString();
                            }
                            // Show be able to show as communicated event if we are not creating a manifest
                            if (res.Success && showAsCommunicated)
                            {
                                // mark the instructions as communicated so that the instruction shows as in progress
                                CommunicateInstructions(jobID, driverResourceID, vehicleResourceID, userName);
                            }
                        }

                        #endregion
                    }

                    #endregion

                    tran.Complete();
                    
                }
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }

            return retVal;
        }

        private static bool CommunicateInstructions(int jobID, int driverID, int vehicleID, string userId)
        {
            Entities.DriverCommunication communication = new Entities.DriverCommunication();
            communication.Comments = "Communicated via Collections screen";
            communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;
            communication.NumberUsed = "unknown";

            Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
            communication.DriverCommunicationType = facDriverCommunication.GetDefaultCommunicationType(driverID, vehicleID);
            communication.DriverCommunicationId = facDriverCommunication.Create(jobID, driverID, communication, userId);

            return true;
        }

        private static bool CommunicateInstructionsForSubContractor(int jobID, int subContractorId, string userId)
        {
            if (!Globals.Configuration.SubContractorCommunicationsRequired)
                return false;

            Entities.DriverCommunication communication = new Entities.DriverCommunication();
            communication.Comments = "Communicated via Deliveries Screen";
            communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;
            string mobileNumber = "unknown";

            communication.DriverCommunicationType = eDriverCommunicationType.Manifest;
            communication.NumberUsed = mobileNumber;

            Facade.IJobSubContractor facJob = new Facade.Job();
            communication.DriverCommunicationId = facJob.CreateCommunication(jobID, subContractorId, communication, userId);

            return true;
        }

        #endregion

        #region Generate Reports

        [System.Web.Services.WebMethod]
        public static string ShowSubbyManfest(string resourceManifestID, bool excludeFirstRow, bool usePlannedTimes, string extraRows, bool showFullAddress, string jobID)
        {
            int rmID, eRows;
            int.TryParse(resourceManifestID, out rmID);
            int.TryParse(extraRows, out eRows);

            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            Entities.ResourceManifest rm = facResourceManifest.GetResourceManifest(rmID);

            // Retrieve the resource manifest 
            NameValueCollection reportParams = new NameValueCollection();
            DataSet manifests = new DataSet();
            manifests.Tables.Add(ManifestGeneration.GetSubbyManifest(rmID, rm.SubcontractorId.Value, usePlannedTimes, excludeFirstRow, showFullAddress, true));

            if (manifests.Tables[0].Rows.Count > 0)
            {
                // Add blank rows if applicable
                if (eRows > 0)
                    for (int i = 0; i < eRows; i++)
                    {
                        DataRow newRow = manifests.Tables[0].NewRow();
                        manifests.Tables[0].Rows.Add(newRow);
                    }

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                reportParams.Add("ManifestName", rm.Description);
                reportParams.Add("ManifestID", rm.ResourceManifestId.ToString());
                reportParams.Add("UsePlannedTimes", usePlannedTimes.ToString());
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = manifests;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "";
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            }

            return string.Format("{0}", jobID);
        }

        [System.Web.Services.WebMethod]
        public static string GenerateAndShowManifest(string resourceManifestID, bool excludeFirstRow, bool usePlannedTimes, string extraRows, bool showFullAddress, string jobID)
        {
            int rmID, eRows;
            int.TryParse(resourceManifestID, out rmID);
            int.TryParse(extraRows, out eRows);

            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
            rm = facResourceManifest.GetResourceManifest(rmID);

            // Retrieve the resource manifest 
            NameValueCollection reportParams = new NameValueCollection();
            DataSet manifests = new DataSet();
            manifests.Tables.Add(ManifestGeneration.GetDriverManifest(rm.ResourceManifestId, rm.ResourceId, usePlannedTimes, excludeFirstRow, showFullAddress, true));

            if (manifests.Tables[0].Rows.Count > 0)
            {
                // Add blank rows if applicable
                if (eRows > 0)
                    for (int i = 0; i < eRows; i++)
                    {
                        DataRow newRow = manifests.Tables[0].NewRow();
                        manifests.Tables[0].Rows.Add(newRow);
                    }


                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                reportParams.Add("ManifestName", rm.Description);
                reportParams.Add("ManifestID", rm.ResourceManifestId.ToString());
                reportParams.Add("UsePlannedTimes", usePlannedTimes.ToString());

                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = manifests;
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "";
                HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            }

            return string.Format("{0}", jobID);
        }

        [System.Web.Services.WebMethod]
        public static string GenerateAndShowLoadingSheet(string jobIDs)
        {
            Facade.IJob facJob = new Facade.Job();
            NameValueCollection reportParams = new NameValueCollection();
            List<int> localJobID = new List<int>();

            localJobID.Add(int.Parse(jobIDs));
            DataSet dsLSS = facJob.GetLoadingSummarySheet(localJobID);

            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.LoadingSummarySheet;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsLSS;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            HttpContext.Current.Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            return string.Empty;
        }

        #endregion

        //------------------------------------------------------------------------------

        #region Page Methods for Update Job

        [System.Web.Services.WebMethod]
        public static string GetOrdersForJob(int jobID)
        {
            List<miniOrder> retVal = new List<miniOrder>();

            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetForJob(jobID);

            miniOrder miniOrder;
            foreach (DataRow row in dsOrders.Tables[0].Rows)
            {
                miniOrder = new miniOrder()
                {
                    Customer = row["Customer"].ToString(),
                    CustomerIdentityID = (int)row["CustomerIdentityID"],
                    CustomerOrderNumber = row["CustomerOrderNumber"].ToString(),
                    DeliveryOrderNumber = row["DeliveryOrderNumber"].ToString(),
                    NoPallets = (int)row["NoPallets"],
                    Weight = (decimal)row["Weight"],
                    OrderID = (int)row["OrderID"],
                    DeliveryPoint = row["DeliveryPoint"].ToString(),
                    CollectionPoint = row["CollectionPoint"].ToString(),
                    OrderStatusID = (int)row["OrderStatusID"]
                };
                retVal.Add(miniOrder);
            }

            // put this into the table format for the display
            string imgRemove = @"<img src=""/App_Themes/Orchestrator/img/masterpage/icon-cross.png"" id=""imgRemove"" style=""cursor:hand; margin: 2px 2px 0 0;""/>";
            string txtOrder = @"<input type=""text"" id=""txtOrder"" value=""{0}"" style=""width:15px; color:Black;"" {1}  />";
            string tr = @"<tr class=""DataGridListItem"" style=""background-color:#949fd4;"">
                    <td  style=""width:40px;"">{5}{6}</td>
                    <td  style=""width:15px;"" class=""orderID""><span id=""orderID"" jobID=""{7}"">{0}</span></td>
                    <td  style=""display:none;""><span id=""collectionPoint"">{1}</span></td>
                    <td  style=""width:135px;""><span id=""deliveryPoint"">{2}</span></td>
                    <td  style=""width:15px;""><span id=""palletSpaces"">{3}</span></td>
                    <td  style=""width:15px;""><span id=""weight"">{4}</span></td>
                </tr>";
            StringBuilder sb = new StringBuilder();
            int dropOrder = 1;
            string ordertextBox = string.Empty;
            foreach (miniOrder o in retVal)
            {

                if (o.OrderStatusID == (int)eOrderStatus.Approved)
                {
                    ordertextBox = string.Format(txtOrder, dropOrder, "");
                    sb.Append(string.Format(tr, o.OrderID, o.CollectionPoint, o.DeliveryPoint, o.NoPallets, ((int)o.Weight).ToString(), imgRemove, ordertextBox, jobID));
                }
                else
                {
                    ordertextBox = string.Format(txtOrder, dropOrder, "\"disabled=disabled\"");
                    sb.Append(string.Format(tr, o.OrderID, o.CollectionPoint, o.DeliveryPoint, o.NoPallets, ((int)o.Weight).ToString(), string.Empty, ordertextBox, 0));
                }
                dropOrder++;
            }

            return sb.ToString();
        }
        #endregion

        //------------------------------------------------------------------------------

        [System.Web.Services.WebMethod]
        public static string GetInstructionsForJob(int jobID)
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(jobID, true);

            StringBuilder sb = new StringBuilder();
            string tr = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>";

            string chk = @"<input type=""checkbox"" id=""chkAddToInstruction{0}"" instructionID=""{0}"" onclick=""SelectLoadInstruction(this)"" />";
            string subTable = @"<table id=""subTableOrders"" instructionID=""{0}"" style=""display:none; width:100%""><tr><td><img src=""/App_Themes/Orchestrator/img/masterpage/icon-cross.png"" id=""imgRemove"" style=""cursor:hand; margin: 2px 2px 0 0;"" onclick=""RemoveOrderFromInstruction(this);""/></td><td><span id=""orderID""></span></td></tr></td></tr></table>";

            string imgload = @"<img src=""/images/loadfinal.png"" height=""16"" width=""16""/>";
            string imgdrop = @"<img src=""/images/dropfinal.png"" height=""16"" width=""16""/>";
            string imgtrunk = @"<img src=""/images/trunk.gif"" height=""16"" width=""16""/>";
            string img = string.Empty;

            foreach (Entities.Instruction i in job.Instructions)
            {
                eInstructionType instructionType = (eInstructionType)i.InstructionTypeId;
                switch (instructionType)
                {
                    case eInstructionType.Load:
                    case eInstructionType.PickupPallets:
                        img = imgload;
                        break;
                    case eInstructionType.Drop:
                    case eInstructionType.LeavePallets:
                    case eInstructionType.DeHirePallets:
                        img = imgdrop;
                        break;
                    case eInstructionType.Trunk:
                        img = imgtrunk;
                        break;
                }

                if (i.InstructionTypeId == (int)eInstructionType.Load && (i.InstructionState == eInstructionState.Booked || i.InstructionState == eInstructionState.Planned))
                {
                    sb.Append(string.Format(tr, string.Format(chk, i.InstructionID), img, i.Point.Description, i.PlannedArrivalDateTime.ToString("dd/MM/yy HH:mm")));
                    sb.Append(string.Format("<tr><td></td><td colspan=\"3\" aling=\"left\">{0}</td></tr>", string.Format(subTable, i.InstructionID)));
                }
                else
                {
                    sb.Append(string.Format(tr, "", img, i.Point.Description, i.PlannedArrivalDateTime.ToString("dd/MM/yy HH:mm")));
                }
            }

            return sb.ToString();
        }

        //------------------------------------------------------------------------------

        [System.Web.Services.WebMethod]
        public static string RemoveOrderFromJob(int jobID, int orderID, string userName, int userID)
        {
            //call remove order 
            string retVal = string.Empty;
            Entities.FacadeResult facResult = null;
            try
            {
                Facade.IJob facJob = new Facade.Job();
                Entities.Job job = facJob.GetJob(jobID, true);

                // if this is the only order on the job then cancel(delete the job);
                if (job.Instructions.GetForInstructionType(eInstructionType.Load).Count == 1 && job.Instructions.GetForInstructionType(eInstructionType.Load)[0].CollectDrops.Count == 1)
                {
                    if (job.Instructions.GetForInstructionType(eInstructionType.Load)[0].CollectDrops[0].OrderID == orderID)
                    {
                        retVal = facJob.UpdateState(jobID, eJobState.Cancelled, userName).ToString();
                        retVal += ",reload";
                    }
                }
                else
                {
                    facResult = facJob.RemoveOrder(jobID, orderID, userName);
                    retVal = facResult.Success.ToString() + ",";
                }
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }

            return retVal;
        }

        //------------------------------------------------------------------------------

        [System.Web.Services.WebMethod]
        public static string UpdateJob(int jobID, List<int> orderIDs, int deliveryPointID, DateTime? trunkDeliveryDate, DateTime? trunkDeliveryTime, int? instructionID, string userName, int userIdentityID, bool updateExistingManifest, DateTime? manifestDate, string manifestTitle, string resourceName)
        {
            string retval = String.Empty;

            try
            {
                #region Prepare Orders
                // build the orders csv
                string orders = string.Empty;
                foreach (int i in orderIDs)
                {
                    if (orders.Length > 0)
                        orders += ",";

                    orders += i.ToString();
                }

                // get the orders 
                Facade.IOrder facorder = new Facade.Order();
                DataSet OrderData = facorder.GetOrdersForList(orders, true, false);

                // Ensure that the Orders are sorted in delivery Order
                DataTable dtOrders = OrderData.Tables[0];
                dtOrders.PrimaryKey = new DataColumn[] { dtOrders.Columns["OrderID"] };
                dtOrders.Columns.Add("CollectionOrder", typeof(int));
                dtOrders.Columns.Add("DeliverToPointID", typeof(int));
                dtOrders.Columns.Add("DeliverAtDateTime", typeof(DateTime));
                dtOrders.Columns.Add("DeliverAtAnyTime", typeof(bool));

                DataRow r = null;
                for (int i = 0; i < orderIDs.Count; i++)
                {
                    // we are using the ordinal position of the orderid in the list to denote the delivery order
                    r = dtOrders.Select("OrderID =" + orderIDs[i])[0];
                    r["CollectionOrder"] = i;
                    r.AcceptChanges();
                }

                // If we are planning the deliveries before the collection and are planning to pick these up from a cross dock location
                // set the relevant information
                if (deliveryPointID > 0)
                {
                    DateTime deliveryDateTime = trunkDeliveryDate.Value;
                    if (trunkDeliveryTime.HasValue)
                        deliveryDateTime = deliveryDateTime.Add(trunkDeliveryTime.Value.TimeOfDay);

                    foreach (DataRow orderRow in dtOrders.Rows)
                    {
                        orderRow["DeliverToPointID"] = deliveryPointID;
                        orderRow["DeliverAtDateTime"] = deliveryDateTime;
                        orderRow["DeliverAtAnyTime"] = !trunkDeliveryTime.HasValue;
                        orderRow.AcceptChanges();
                    }
                }
                else
                    foreach (DataRow orderRow in dtOrders.Rows)
                    {
                        orderRow["DeliverToPointID"] = orderRow["DeliveryPointID"];
                        orderRow["DeliverAtDateTime"] = orderRow["DeliveryDateTime"];
                        orderRow["DeliverAtAnyTime"] = orderRow["DeliveryIsAnyTime"];
                        orderRow.AcceptChanges();
                    }

                #endregion

                #region Update the Job

                // Load the job
                Facade.IJob facJob = new Facade.Job();
                Entities.Job job = facJob.GetJob(jobID, true);
                Entities.FacadeResult facResult = null;

                Entities.InstructionCollection collections = job.Instructions.GetForInstructionType(eInstructionType.Load);
                List<Entities.Instruction> amendedCollections = new List<Orchestrator.Entities.Instruction>();
                Entities.Instruction iCollect = null;
                Entities.CollectDrop cd = null;

                Entities.InstructionCollection drops = null;
                if (deliveryPointID > 0)
                {
                    drops = job.Instructions.GetForInstructionType(eInstructionType.Trunk);
                }
                else
                    drops = job.Instructions.GetForInstructionType(eInstructionType.Drop);

                List<Entities.Instruction> amendedDrops = new List<Orchestrator.Entities.Instruction>();
                Entities.Instruction iDrop = null;

                Facade.IPoint facPoint = new Facade.Point();
                Facade.IOrder facOrder = new Facade.Order();
                Entities.Point point = null;

                bool newcollection = false;
                foreach (DataRow row in OrderData.Tables[0].Rows)
                {
                    #region Collections
                    newcollection = false;
                    int collectionPointID = (int)row["CollectionPointID"];
                    DateTime bookedDateTime = (DateTime)row["CollectionDateTime"];
                    bool collectionIsAnytime = (bool)row["CollectionIsAnyTime"];

                    // if this setting is true then we want to create a new instruction for the order.
                    if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                        iCollect = null;
                    else
                        if (instructionID.HasValue)
                        {
                            // we are going to add this to an existing load instruction
                            iCollect = collections.GetForInstructionId(instructionID.Value);
                        }
                        else
                        {
                            iCollect = collections.GetForInstructionTypeAndPoint(eInstructionType.Load, collectionPointID);
                        }

                    if (iCollect == null)
                    {
                        iCollect = new Orchestrator.Entities.Instruction();
                        iCollect.InstructionTypeId = (int)eInstructionType.Load;
                        iCollect.BookedDateTime = bookedDateTime;
                        if ((bool)row["CollectionIsAnyTime"])
                            iCollect.IsAnyTime = true;
                        point = facPoint.GetPointForPointId(collectionPointID);
                        iCollect.PointID = collectionPointID;
                        iCollect.Point = point;
                        iCollect.ClientsCustomerIdentityID = point.IdentityId; //Steve is this correct
                        iCollect.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                        newcollection = true;
                    }

                    cd = new Orchestrator.Entities.CollectDrop();
                    cd.NoPallets = (int)row["NoPallets"];
                    cd.PalletTypeID = (int)row["PalletTypeID"];
                    cd.NoCases = (int)row["Cases"];
                    cd.GoodsTypeId = (int)row["GoodsTypeID"];
                    cd.OrderID = (int)row["OrderID"];
                    cd.Weight = (decimal)row["Weight"];
                    cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                    cd.Docket = row["OrderID"].ToString();

                    iCollect.CollectDrops.Add(cd);
                    amendedCollections.Add(iCollect);

                    #endregion
                }

                // Add the Drops in the Order specified
                DataView dvDrops = OrderData.Tables[0].DefaultView;
                dvDrops.Sort = "CollectionOrder DESC";
                DataTable dtDrops = dvDrops.ToTable();
                foreach (DataRow row in dtDrops.Rows)
                {
                    #region Deliveries
                    bool newdelivery = false;
                    newdelivery = false;
                    int pointId = (int)row["DeliverToPointID"];
                    DateTime deliveryDateTime = (DateTime)row["DeliverAtDateTime"];
                    bool deliveryIsAnyTime = (bool)row["DeliverAtAnyTime"];

                    // if this setting is true then we want to create a new instruction for the order.
                    if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                        iDrop = null;
                    else
                        //iDrop = drops.GetForInstructionTypeAndPoint(eInstructionType.Drop, pointId);
                        iDrop = drops.FirstOrDefault(i => i.PointID == pointId);

                    if (iDrop == null)
                    {
                        iDrop = new Orchestrator.Entities.Instruction();
                        if (deliveryPointID > 0)
                            iDrop.InstructionTypeId = (int)eInstructionType.Trunk;
                        else
                            iDrop.InstructionTypeId = (int)eInstructionType.Drop;

                        iDrop.BookedDateTime = deliveryDateTime;
                        if ((bool)row["DeliverAtAnyTime"])
                            iDrop.IsAnyTime = true;
                        point = facPoint.GetPointForPointId(pointId);
                        iDrop.ClientsCustomerIdentityID = point.IdentityId;
                        iDrop.PointID = pointId;
                        iDrop.Point = point;

                        iDrop.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                        newdelivery = true;
                    }

                    cd = new Orchestrator.Entities.CollectDrop();
                    cd.NoPallets = (int)row["NoPallets"];
                    cd.PalletTypeID = (int)row["PalletTypeID"];
                    cd.NoCases = (int)row["Cases"];
                    cd.GoodsTypeId = (int)row["GoodsTypeID"];
                    cd.OrderID = (int)row["OrderID"];
                    cd.Weight = (decimal)row["Weight"];
                    cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                    cd.Docket = row["OrderID"].ToString();

                    iDrop.CollectDrops.Add(cd);
                    amendedDrops.Add(iDrop);

                    #endregion
                }
                List<Entities.Instruction> amendedInstructions = new List<Entities.Instruction>();
                amendedInstructions.AddRange(amendedCollections);
                amendedInstructions.AddRange(amendedDrops);

                facResult = facJob.AmendInstructions(job, amendedInstructions, eLegTimeAlterationMode.Minimal, true, null, userName);

                #endregion

                if (updateExistingManifest)
                {
                    #region Update ResourceManif3sts

                    bool updateManifest = false, singleResource = false;

                    // get the resource manifest ID for this job and resource;
                    Facade.ResourceManifest facmanifest = new Orchestrator.Facade.ResourceManifest();
                    List<Entities.ResourceManifest> existingManifests = facmanifest.GetManifestsForRun(job.JobId);

                    if (existingManifests.Count <= 1)
                    {
                        if (existingManifests.Count == 1)
                            updateManifest = true;

                        // Get the job in its current state, including resource.
                        job = facJob.GetJob(jobID, true, true);

                        int instructionCount = job.Instructions.Count;
                        int subContractorID = -1, driverID = -1;

                        // If the job is subbed out whole job then its ok.
                        if (job.SubContractors != null && job.SubContractors.Count > 0 && job.SubContractors.First().SubContractWholeJob)
                        {
                            subContractorID = job.SubContractors.First().ContractorIdentityId;
                            singleResource = true;
                        }

                        // If the job is resourced using the same driver for all instructions then its ok.
                        if (!singleResource && job.Instructions.Exists(i => i.Driver != null && i.Driver.ResourceId > 0))
                        {
                            driverID = job.Instructions.Find(i => i.Driver.ResourceId > 0).Driver.ResourceId;
                            ReadOnlyCollection<Entities.Instruction> foundInstructions = job.Instructions.FindAll(i => i.Driver != null && i.Driver.ResourceId == driverID);

                            if (foundInstructions.Count == instructionCount)
                                singleResource = true;
                        }

                        // If a single driver or subby is used throughout then proceed.
                        if (singleResource)
                        {
                            Entities.ResourceManifest rm = new Entities.ResourceManifest();

                            // If there is an existing on, amend it otherwise create a new manifest.
                            if (updateManifest)
                                rm = existingManifests.First();
                            else
                            {
                                rm.ManifestDate = manifestDate ?? DateTime.Today;

                                if (driverID > 0)
                                    rm.ResourceId = driverID;
                                else
                                    rm.SubcontractorId = subContractorID;

                                if (!string.IsNullOrEmpty(manifestTitle))
                                    rm.Description = manifestTitle;
                                else
                                    rm.Description = resourceName + " - " + rm.ManifestDate.ToString("dd/MM/yy");
                            }

                            int jobOrder = 0;
                            List<ResourceManifestJob> newrmj = new List<ResourceManifestJob>();

                            foreach (Entities.Instruction ci in job.Instructions)
                            {
                                ResourceManifestJob rmj = null;

                                if (updateManifest)
                                    rmj = rm.ResourceManifestJobs.FirstOrDefault(crmj => crmj.InstructionId == ci.InstructionID);

                                if (rmj == null)
                                {
                                    rmj = new ResourceManifestJob();
                                    rmj.JobId = ci.JobId;
                                    rmj.InstructionId = ci.InstructionID;
                                    rmj.JobOrder = jobOrder;

                                    if (updateManifest)
                                        rmj.ResourceManifestId = rm.ResourceManifestId;
                                }
                                else
                                    rmj.JobOrder = jobOrder;

                                newrmj.Add(rmj);
                                jobOrder++;
                            }

                            rm.ResourceManifestJobs = newrmj;

                            if (updateManifest)
                                facmanifest.UpdateResourceManifest(rm, userName);
                            else
                                rm.ResourceManifestId = facmanifest.CreateResourceManifest(rm, userName);

                            if (rm.SubcontractorId != null)
                                retval = string.Format("{0},1,{1},1", job.JobId.ToString(), rm.ResourceManifestId.ToString()); // Subby Manifest
                            else
                                retval = string.Format("{0},1,{1}", job.JobId.ToString(), rm.ResourceManifestId.ToString()); // Driver Manifest
                        }
                        else
                            retval = string.Format("{0},0", job.JobId.ToString()); // Manifest could not be created, force's popup link.
                    }

                    #endregion
                }
                else
                    retval = string.Format("{0}", job.JobId.ToString()); // No Manifest requested.
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
            return retval;
        }

        //------------------------------------------------------------------------------

        #region Page methods for Booking In

        [System.Web.Services.WebMethod]
        public static bool BookIn(int orderID, string bookedInBy)
        {
            Orchestrator.EF.DataContext context = new Orchestrator.EF.DataContext();
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);
            order.BookedIn = true;
            order.BookedInByUserName = bookedInBy;
            order.BookedInDateTime = DateTime.Now;
            order.BookedInStateId = (int)eBookedInState.BookedIn;
            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = bookedInBy;
            context.SaveChanges(true);

            return true;
        }
        #endregion

        //------------------------------------------------------------------------------

        #region Page Method for Changing order business types

        [System.Web.Services.WebMethod]
        public static bool UpdateBusinessType(string orderIDCSV, int businessTypeID, string userName)
        {
            Facade.IOrder facOrder = new Facade.Order();
            return facOrder.UpdateOrdersBusinessType(orderIDCSV, businessTypeID, userName);
        }

        #endregion

        //------------------------------------------------------------------------------

        [Serializable()]
        public struct miniOrder
        {
            public int OrderID;
            public int CustomerIdentityID;
            public string Customer;
            public string CustomerOrderNumber;
            public string DeliveryOrderNumber;
            public int NoPallets;
            public decimal Weight;

            public string DeliveryCustomer;
            public string DeliveryPoint;
            public string CollectionPoint;
            public int OrderStatusID;

            //------------------------------------------------------------------------------

            public miniOrder(int orderID, int customerIdentityID, string customer, string customerOrderNumber, string deliveryOrderNumber, int noPallets, decimal weight)
            {
                OrderID = orderID;
                Customer = customer;
                CustomerIdentityID = customerIdentityID;
                CustomerOrderNumber = customerOrderNumber;
                DeliveryOrderNumber = deliveryOrderNumber;
                NoPallets = noPallets;
                Weight = weight;
                DeliveryCustomer = string.Empty;
                DeliveryPoint = string.Empty;
                CollectionPoint = string.Empty;
                OrderStatusID = -1;
            }

            //------------------------------------------------------------------------------

        }

        //------------------------------------------------------------------------------

    }

    //------------------------------------------------------------------------------

}
