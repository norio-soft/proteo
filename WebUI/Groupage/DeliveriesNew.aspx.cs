using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using Telerik.Web.UI;
using System.Xml.Serialization;
using System.Collections.Specialized;

using Orchestrator.BusinessLogicLayer;
using Orchestrator.Entities;
using System.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;
using Orchestrator.WebUI;
using System.Transactions;

namespace Orchestrator.WebUI.Groupage
{
    public partial class DeliveriesNew : Orchestrator.Base.BasePage
    {
        #region Private fields

        private enum eOrderGroupPosition { Single, First, Mid, Last };

        private bool _exportOrderEnabled = false;
        private int _noPallets = 0;
        private decimal _noPalletSpaces = 0;
        private decimal _weight = 0.0M;
        private string _orders = string.Empty;
        private List<int> _jobIDs = null;
        private List<int> _ordersList;
        private readonly string VS_ROWFILTER = "_rowFilter";
        private const string C_GRID_NAME = "__deliveriesGrid";
        private const string C_SELECTED_SERVICE_LEVELS = "SelectedServiceLevels";
        private const string C_SELECTED_SURCHARGES = "SelectedSurcharges";
        private const eDateFilterType C_DEFAULT_DATE_FILTERING = eDateFilterType.Delivery_Date;
        private const string C_SELECTED_BOOKIN_REQUIRED = "SelectedBookInRequired";
        private const string C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN = "SelectedDoesNotRequireBookingIn";
        private const string C_SELECTED_BOOKEDIN = "SelectedBookedIn";
        private const string C_SELECTED_DATE_FILTERING = "SelectedDateFiltering";
        private const string C_SELECTED_SHOW_EMPTY_TRUNK_ROWS = "SelectedShowEmptyTrunkRows";
        private const string C_SELECTED_SHOW_NOTPLANNEDFORDELIVERY = "SelectedShowNotPlannedForDelivery";
        private const string C_SELECTED_SHOW_ALL = "SelectedShowAll";
        private const string C_SELECTED_START_GRID_COLLAPSED = "SelectedStartGridCollapsed";
        private const string C_SELECTED_DELIVERIES_CLIENT = "SelectedDeliveriesCLient";
        private const string C_SELECTED_COLLECTION_POINT = "SelectedCollectionPoint";
        private bool _updateGridSettings = false;
        #endregion

        #region properties

        private string RowFilter
        {
            get { return (string)this.ViewState[VS_ROWFILTER]; }
            set { this.ViewState[VS_ROWFILTER] = value; }
        }

        public List<int> OrdersList
        {
            get
            {

                if (_ordersList == null)
                    _ordersList = new List<int>();

                _ordersList.Clear();
                int orderID = 0;
                foreach (GridItem row in grdDeliveries.Items)
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

        public string Orders
        {
            get
            {
                _orders = string.Empty;
                int orderID = 0;
                foreach (GridItem row in grdDeliveries.Items)
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

        private List<int> JobIDs
        {
            get
            {
                _jobIDs = new List<int>();
                int jobID = 0;
                foreach (GridItem row in grdDeliveries.Items)
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

        protected int NoPallets
        {
            get
            {
                _noPallets = 0;

                foreach (GridDataItem row in grdDeliveries.Items)
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

        protected decimal NoPalletSpaces
        {
            get
            {
                _noPalletSpaces = 0;

                foreach (GridDataItem row in grdDeliveries.Items)
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

        protected decimal Weight
        {
            get
            {
                _weight = 0;

                foreach (GridDataItem row in grdDeliveries.Items)
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

        public List<int> ServiceLevelIDs
        {
            get
            {
                List<int> serviceLevelIDs = new List<int>();
                int holding = 0;
                foreach (ListItem li in cblServiceLevel.Items)
                    if (li.Selected && int.TryParse(li.Value, out holding) && !serviceLevelIDs.Contains(holding))
                        serviceLevelIDs.Add(holding);

                return serviceLevelIDs;
            }
        }

        public List<int> SurchargeIDs
        {
            get
            {
                List<int> surchargeIDs = new List<int>();
                int holding = 0;
                foreach (ListItem li in cblSurcharges.Items)
                    if (li.Selected && int.TryParse(li.Value, out holding) && !surchargeIDs.Contains(holding))
                        surchargeIDs.Add(holding);

                return surchargeIDs;
            }
        }

        public List<int> TrafficAreaIDs
        {
            get
            {
                List<int> trafficAreaIDs = new List<int>();
                int holding = 0;
                foreach (ListItem li in cblTrafficAreas.Items)
                    if (li.Selected && int.TryParse(li.Value, out holding) && !trafficAreaIDs.Contains(holding))
                        trafficAreaIDs.Add(holding);

                return trafficAreaIDs;
            }
        }

        private Dictionary<int, string> _surchargeLookupData = new Dictionary<int, string>();
        public IDictionary<int, string> SurchargeLookupData
        {
            get { return _surchargeLookupData; }
        }

        public int PalletNetworkBusinessTypeID { get; private set; }
        #endregion

        #region Moved the viewstate to the server for optimisation
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this.Page);
            }
        }
        #endregion

        #region page Load/Init

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
                //    bool showFullAddress = (Request.QueryString["showFullAddress"] == null) ? false : bool.Parse(Request.QueryString["showFullAddress"]);

                //    if (!String.IsNullOrEmpty(Request.QueryString["isSubby"]))
                //        ShowSubbyManifest(int.Parse(Request.QueryString["rm"]), excludeFirstline, usePlannedTimes, extraRows);
                //    else
                //        GenerateAndShowManifest(int.Parse(Request.QueryString["rm"]), excludeFirstline, extraRows, usePlannedTimes, showFullAddress);
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

                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
                cblTrafficAreas.DataSource = facTrafficArea.GetAll();
                cblTrafficAreas.DataBind();

                foreach (ListItem item in cblTrafficAreas.Items)
                {
                    item.Attributes.Add("onclick", "onTrafficAreaChecked(this);");
                }

                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                DataSet dsBusinessTypes = facBusinessType.GetAll();
                PalletNetworkBusinessTypeID = -1;
                bool palletNetworkFound = false;

                foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                {
                    RadMenuItem rmi = new RadMenuItem();
                    rmi.Text = row["Description"].ToString();
                    rmi.Value = row["BusinessTypeID"].ToString();
                    if ((bool)row["IsPalletNetwork"] && !palletNetworkFound)
                    {
                        PalletNetworkBusinessTypeID = (int)row["BusinessTypeID"];
                        palletNetworkFound = true;
                    }
                    RadMenu1.Items.Add(rmi);
                }

                Facade.IOrderServiceLevel facOrderServiceLevel = new Facade.Order();
                cblServiceLevel.DataSource = facOrderServiceLevel.GetAll();
                cblServiceLevel.DataBind();

                Facade.ExtraType facExtraType = new Facade.ExtraType();
                cblSurcharges.DataSource = facExtraType.GetAllForFiltering();
                cblSurcharges.DataBind();


                GetDates();

                Facade.IOrganisation facOrg = new Facade.Organisation();
                Entities.Organisation org = facOrg.GetForIdentityId(Globals.Configuration.IdentityId);
                Facade.IPoint facPoint = new Facade.Point();
                Entities.Point deliveryRunCollectionPoint = null;
                if (org.Defaults[0].GroupageDeliveryRunCollectionPoint > 0)
                {
                    deliveryRunCollectionPoint = facPoint.GetPointForPointId(org.Defaults[0].GroupageDeliveryRunCollectionPoint);
                    ucCollectionPoint.SelectedPoint = deliveryRunCollectionPoint;
                }

                cblGridColumns.Items.FindByValue("DeliveryOrderNumber").Text = grdDeliveries.Columns.FindByUniqueName("DeliveryOrderNumber").HeaderText = Globals.Configuration.SystemDocketNumberText;
                cblGridColumns.Items.FindByValue("CustomerOrderNumber").Text = grdDeliveries.Columns.FindByUniqueName("CustomerOrderNumber").HeaderText = Globals.Configuration.SystemLoadNumberText;

                // get the filter options from the cookie if it exists
                ConfigureDisplay();

                ConfigureOrganisationReferences();

                LoadGridSettings();
                StoreFilterOptions();

                // Wire up the behaviours for the booked in date time options
                rimDeliveries.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromDate.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromDate.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromTime.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromTime.UniqueID, true));

                txtBookInFromDate.Text = DateTime.Today.AddDays(1).ToString("dd/MM/yy");
                txtBookInFromTime.Text = "08:00";
                txtBookInByFromDate.Text = DateTime.Today.AddDays(1).ToString("dd/MM/yy");
                txtBookInByFromTime.Text = "17:00";
                dteTrunkDate.SelectedDate = DateTime.Today;

                if (!Globals.Configuration.PodLabelsEnabled)
                {
                    this.chkPrintPodLabels.Visible = false;
                    this.lblPrintPodLabels.Visible = false;
                }
                else
                {
                    this.chkPrintPodLabels.Visible = true;
                    this.lblPrintPodLabels.Visible = true;
                }

                rbAllocation.Visible = Utilities.IsAllocationEnabled();

                chkTrunkCollection.Checked = Globals.Configuration.LoadBuilderDefaultPickFromCrossDock;
            }
            else
            {
                rimDeliveries.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromDate.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromDate.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromTime.UniqueID, true));
                rimDeliveries.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromTime.UniqueID, true));
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (_updateGridSettings)
                SaveGridSettings(false);
        }

        private void ConfigureDisplay()
        {
            if (Request.Cookies["Deliveries"] != null)
            {
                HttpCookie deliveriesCookie = Request.Cookies["Deliveries"];

                if (deliveriesCookie[C_SELECTED_SURCHARGES] != null)
                {
                    List<string> surcharges = deliveriesCookie[C_SELECTED_SURCHARGES].ToString().Split(',').ToList();
                    PreSelectItems(cblSurcharges, surcharges);
                }
                if (deliveriesCookie[C_SELECTED_SERVICE_LEVELS] != null)
                {
                    List<string> serviceLevels = deliveriesCookie[C_SELECTED_SERVICE_LEVELS].ToString().Split(',').ToList();
                    PreSelectItems(cblServiceLevel, serviceLevels);
                }

                if (deliveriesCookie[C_SELECTED_DATE_FILTERING] != null)
                    rblDateFiltering.SelectedValue = deliveriesCookie[C_SELECTED_DATE_FILTERING];
                
                if (deliveriesCookie[C_SELECTED_SHOW_NOTPLANNEDFORDELIVERY] != null)
                    chkShowNotPlanned.Checked = bool.Parse(deliveriesCookie[C_SELECTED_SHOW_NOTPLANNEDFORDELIVERY]);
                
                if (deliveriesCookie[C_SELECTED_SHOW_ALL] != null)
                {
                    var showAll = bool.Parse(deliveriesCookie[C_SELECTED_SHOW_ALL]);
                    chkShowAll.Checked = showAll;
                    chkShowNotPlanned.Enabled = !showAll;

                    if (showAll)
                        chkShowNotPlanned.Checked = true;
                }

                if (deliveriesCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS] != null)
                    chkNoTrunkDate.Checked = bool.Parse(deliveriesCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS]);
                if (deliveriesCookie[C_SELECTED_BOOKIN_REQUIRED] != null)
                    chkRequiresBookingIn.Checked = bool.Parse(deliveriesCookie[C_SELECTED_BOOKIN_REQUIRED]);
                if (deliveriesCookie[C_SELECTED_BOOKEDIN] != null)
                    chkBookedIn.Checked = bool.Parse(deliveriesCookie[C_SELECTED_BOOKEDIN]);
                if (deliveriesCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN] != null)
                    chkDoesNotRequireBookingIn.Checked = bool.Parse(deliveriesCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN]);
                if (deliveriesCookie[C_SELECTED_START_GRID_COLLAPSED] != null)
                    chkStartCollapsed.Checked = bool.Parse(deliveriesCookie[C_SELECTED_START_GRID_COLLAPSED]);

                if (!String.IsNullOrEmpty(deliveriesCookie[C_SELECTED_COLLECTION_POINT]))
                {
                    Entities.Point collectionPointFilter = null;
                    Facade.IPoint facPoint = new Facade.Point();
                    collectionPointFilter = facPoint.GetPointForPointId(Convert.ToInt32(deliveriesCookie[C_SELECTED_COLLECTION_POINT]));
                    this.ucCollectionPointFilter.SelectedPoint = collectionPointFilter;
                }

                if (!String.IsNullOrEmpty(deliveriesCookie[C_SELECTED_DELIVERIES_CLIENT]))
                {
                    //Set the Client combo yo the Client of the User and diable it
                    cboClient.SelectedValue = deliveriesCookie[C_SELECTED_DELIVERIES_CLIENT];
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    int clientId = Convert.ToInt32(deliveriesCookie[C_SELECTED_DELIVERIES_CLIENT]);
                    Entities.Organisation organisation = facOrganisation.GetForIdentityId(clientId);
                    cboClient.Text = organisation.OrganisationName;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdDeliveries.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdDeliveries_NeedDataSource);
            this.grdDeliveries.ItemCommand += new GridCommandEventHandler(grdDeliveries_ItemCommand);
            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnLoadingSummarySheet.Click += new EventHandler(btnLoadingSummarySheet_Click);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.grdDeliveries.DataBound += new EventHandler(grdDeliveries_DataBound);
            this.grdDeliveries.ItemDataBound += new GridItemEventHandler(grdDeliveries_ItemDataBound);
            this.btnSaveGridSettings.Click += new EventHandler(btnSaveGridSettings_Click);
            this.btnChangeColumns.Click += new EventHandler(btnChangeColumns_Click);
            this.btnResetGridLayout.Click += new EventHandler(btnResetGridLayout_Click);
            
        }

        protected void btnConnectionError_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        #endregion

        private void PreSelectItems(CheckBoxList cbl, IList items)
        {
            foreach (var id in items)
            {
                ListItem item = cbl.Items.FindByValue(id.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }

        #region Cookie Handling

        private void GetDates()
        {
            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (_filter != null)
            {
                if (_filter.FilterStartDate == DateTime.MinValue)
                {
                    dteStartDate.SelectedDate = DateTime.Today;
                    dteStartDate.SelectedDate = dteStartDate.SelectedDate.Value.Subtract(dteStartDate.SelectedDate.Value.TimeOfDay);
                    dteEndDate.SelectedDate = DateTime.Today.AddDays(1);
                }
                else
                {
                    dteStartDate.SelectedDate = _filter.FilterStartDate;
                    dteEndDate.SelectedDate = _filter.FilterEnddate;
                }

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

                this.businessTypeCheckList.SelectedBusinessTypeIDs = _filter.BusinessTypes;

                rblDateFiltering.Items.FindByValue(((int)_filter.DeliveryDateFilter).ToString()).Selected = true;
            }
            else
            {
                _filter = GetDefaultFilter();
                if (_filter.FilterStartDate == DateTime.MinValue)
                {
                    _filter.FilterStartDate = DateTime.Today;
                    _filter.FilterEnddate = _filter.FilterStartDate.Add(new TimeSpan(23, 59, 59));
                }
                else
                {
                    dteStartDate.SelectedDate = _filter.FilterStartDate;
                    dteEndDate.SelectedDate = _filter.FilterEnddate;
                }

                dteStartDate.SelectedDate = _filter.FilterStartDate;
                dteEndDate.SelectedDate = _filter.FilterEnddate;

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

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }

        #endregion

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
                    this.grdDeliveries.MasterTableView.Columns.Add(column);
                    column.UniqueName = uniqueName;
                    column.HeaderText = columnDescription;
                    column.Visible = false;
                }
            }
        }

        private void CaptureOrderIDs()
        {
            List<int> orders = new List<int>();
            foreach (GridDataItem gdi in this.grdDeliveries.Items)
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

        #region Event Handlers

        #region Button Events

        void btnChangeColumns_Click(object sender, EventArgs e)
        {
            SaveGridSettings(true);
        }

        void btnLoadingSummarySheet_Click(object sender, EventArgs e)
        {
            List<int> LocalJobIDs = JobIDs;

            if (LocalJobIDs.Count > 0)
            {
                #region Pop-up Report
                Facade.IJob facJob = new Facade.Job();
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

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            // CaptureOrderIDs();
            Entities.TrafficSheetFilter _filter = GetFilter();
            _filter.FilterStartDate = dteStartDate.SelectedDate.Value;
            _filter.FilterEnddate = dteEndDate.SelectedDate.Value;

            if (_filter.FilterEnddate.TimeOfDay != new TimeSpan(23, 59, 59))
                _filter.FilterEnddate = _filter.FilterEnddate.Subtract(_filter.FilterEnddate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

            _filter.BusinessTypes.Clear();
            _filter.BusinessTypes.AddRange(businessTypeCheckList.SelectedBusinessTypeIDs);

            _filter.TrafficAreaIDs.Clear();

            if (cblTrafficAreas.Items.Count > 0)
            {
                foreach (ListItem li in cblTrafficAreas.Items)
                {
                    if (li.Selected)
                        _filter.TrafficAreaIDs.Add(int.Parse(li.Value));
                }

                SetCookie(_filter);

                StoreFilterOptions();

                LoadGridSettings();
                grdDeliveries.Rebind();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "refreshConnection(); ", true);
            }
        }

        public void btnRefresh_Click_NoFilterUpdates(object sender, EventArgs e)
        {
            if (cblTrafficAreas.Items.Count > 0)
            {
                grdDeliveries.Rebind();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "refreshConnection(); ", true);
            }
        }

        private void StoreFilterOptions()
        {
            //TDOD: Make this a permanent storage 
            // Store the selected surcharge and service level ids.
            HttpCookie deliveriesCookie = new HttpCookie("deliveries");
            deliveriesCookie.Expires = DateTime.Today.AddYears(10);
            deliveriesCookie[C_SELECTED_SURCHARGES] = this.SurchargeIDs.ToCSVString();
            deliveriesCookie[C_SELECTED_SERVICE_LEVELS] = this.ServiceLevelIDs.ToCSVString();
            deliveriesCookie[C_SELECTED_BOOKEDIN] = this.chkBookedIn.Checked.ToString();
            deliveriesCookie[C_SELECTED_BOOKIN_REQUIRED] = this.chkRequiresBookingIn.Checked.ToString();
            deliveriesCookie[C_SELECTED_DATE_FILTERING] = this.rblDateFiltering.SelectedValue;
            deliveriesCookie[C_SELECTED_SHOW_EMPTY_TRUNK_ROWS] = this.chkNoTrunkDate.Checked.ToString();
            deliveriesCookie[C_SELECTED_START_GRID_COLLAPSED] = this.chkStartCollapsed.Checked.ToString();
            deliveriesCookie[C_SELECTED_SHOW_NOTPLANNEDFORDELIVERY] = this.chkShowNotPlanned.Checked.ToString();
            deliveriesCookie[C_SELECTED_SHOW_ALL] = this.chkShowAll.Checked.ToString();
            deliveriesCookie[C_SELECTED_BOOKIN_DOES_NOT_REQUIRE_BOOKIN_IN] = this.chkDoesNotRequireBookingIn.Checked.ToString();
            deliveriesCookie[C_SELECTED_DELIVERIES_CLIENT] = this.cboClient.SelectedValue;
            deliveriesCookie[C_SELECTED_COLLECTION_POINT] = this.ucCollectionPointFilter.PointID.ToString();

            this.Response.Cookies.Add(deliveriesCookie);
        }

        Entities.TrafficSheetFilter GetFilter()
        {
            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (_filter == null)
                _filter = GetDefaultFilter();
            return _filter;
        }

        private Entities.TrafficSheetFilter GetDefaultFilter()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            Entities.TrafficSheetFilter ts = facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

            return ts;
        }

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (Orders != string.Empty)
            {
                dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(Orders);

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
                foreach (GridDataItem item in grdDeliveries.Items)
                {
                    CheckBox chkOrderId = (CheckBox)item.FindControl("chkOrderID");
                    int orderId;
                    int.TryParse(chkOrderId.Attributes["OrderID"].ToString(), out orderId);

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

        void btnResetGridLayout_Click(object sender, EventArgs e)
        {
            ResetGridLayout();
            Response.Redirect("DeliveriesNew.aspx?" + this.CookieSessionID);
        }

        #endregion Button Events

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

        private void grdDeliveries_DataBound(object sender, EventArgs e)
        {
            var groups =
                from i in grdDeliveries.Items.Cast<GridDataItem>()
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
            Action<GridDataItem, eOrderGroupPosition, int> setOrderGroupImage = (item, pos, orderGroupId) =>
                {
                    ((PlaceHolder)item.FindControl("plcGroupedOrderImage")).Controls.Add(new Image { SkinID = positionSkinIDs[pos] });
                    ((PlaceHolder)item.FindControl("plcGroupedOrderImage")).Controls.Add(new HtmlAnchor { InnerHtml = orderGroupId.ToString(), HRef = String.Format("javascript:ViewGroup({0});", orderGroupId) });
                };

            foreach (var group in groups)
            {
                var count = group.Count();
                if (count == 1)
                    setOrderGroupImage(group.First(), eOrderGroupPosition.Single, (int)group.First().GetDataKeyValue("OrderGroupID"));
                else
                {
                    setOrderGroupImage(group.First(), eOrderGroupPosition.First, (int)group.First().GetDataKeyValue("OrderGroupID"));
                    foreach (var item in group.Skip(1).Take(count - 2))
                    {
                        setOrderGroupImage(item, eOrderGroupPosition.Mid, (int)group.First().GetDataKeyValue("OrderGroupID"));
                    }
                    setOrderGroupImage(group.Last(), eOrderGroupPosition.Last, (int)group.First().GetDataKeyValue("OrderGroupID"));
                }
            }
        }

        void grdDeliveries_ItemDataBound(object sender, GridItemEventArgs e)
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
                    string deliveryAddressLines = string.Empty;
                    deliveryAddressLines = string.IsNullOrEmpty(drv["AddressLine1"].ToString()) ? "" : drv["AddressLine1"].ToString();
                    deliveryAddressLines += string.IsNullOrEmpty(drv["AddressLine2"].ToString()) ? "" : "<br/>" + drv["AddressLine2"].ToString();
                    deliveryAddressLines += string.IsNullOrEmpty(drv["AddressLine3"].ToString()) ? "" : "<br/>" + drv["AddressLine3"].ToString();
                    deliveryAddressLines += string.IsNullOrEmpty(drv["PostTown"].ToString()) ? "" : "<br/>" + drv["PostTown"].ToString();

                    // Collect At column
                    Label lblCollectAt = (Label)e.Item.FindControl("lblCollectAt");
                    if (lblCollectAt != null)
                    {
                        DateTime colFrom = Convert.ToDateTime(drv["CollectionDateTime"].ToString());
                        DateTime colBy = Convert.ToDateTime(drv["CollectionByDateTime"].ToString());

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

                    // Delivery At column
                    Label lblDeliverAt = (Label)e.Item.FindControl("lblDeliverAt");
                    if (lblDeliverAt != null)
                    {
                        DateTime delFrom = Convert.ToDateTime(drv["DeliveryFromDateTime"].ToString());
                        DateTime delBy = Convert.ToDateTime(drv["DeliveryDateTime"].ToString());

                        if (delFrom == delBy)
                        {
                            // Timed booking... only show a single date.
                            lblDeliverAt.Text = GetDate(Convert.ToDateTime(drv["DeliveryDateTime"].ToString()), false);
                        }
                        else
                        {
                            // If the times span from mignight to 23:59 on the same day then 
                            // it's an 'anytime' window.
                            if (delFrom.Date == delBy.Date && delFrom.Hour == 0 && delFrom.Minute == 0 && delBy.Hour == 23 && delBy.Minute == 59)
                            {
                                // It's anytime
                                lblDeliverAt.Text = GetDate(Convert.ToDateTime(drv["DeliveryDateTime"].ToString()), true);
                            }
                            else
                            {
                                // It's a booking window
                                lblDeliverAt.Text = GetDate(Convert.ToDateTime(drv["DeliveryFromDateTime"].ToString()), false) + " to " + GetDate(Convert.ToDateTime(drv["DeliveryDateTime"].ToString()), false);
                            }
                        }
                    }


                    // OrderID, NoPallets, OrderGroupId, OrderGroupGroupedPlanning, PalletSpaces, Weight, Collection Point, Delivery Point, DropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress
                    string showOrderDetails = "javascript:ChangeList(event, this, {0}, {1}, {2}, {3}, {4}, {5}, '{6}', '{7}', {8}, {9}, '{10}', '{11}', '{12}', {13}, {14}, '{15}');";
                    //"javascript:ChangeList(event, this, {0}, {1}, {2}, {3}, {4}, {5});"
                    if (drv["DeliveryJobID"] == DBNull.Value)
                    {
                        //Encode any spaces etc
                        string deliveryPoint = Server.HtmlEncode(drv["DeliveryPoint"].ToString());
                        string collectionPoint = Server.HtmlEncode(drv["CollectionPoint"].ToString());
                        string customer = Server.HtmlEncode(drv["Customer"].ToString());

                        //Converts apostrophes to ASCII numerical code, will be interpreted by the browser as '
                        deliveryPoint = ConvertApostrophesToASCII(deliveryPoint);
                        collectionPoint = ConvertApostrophesToASCII(collectionPoint);
                        customer = ConvertApostrophesToASCII(customer);
                        deliveryAddressLines = ConvertApostrophesToASCII(deliveryAddressLines);



                        chk.Attributes.Add("OrderGroupID", drv["OrderGroupID"].ToString());
                        chk.Attributes.Add("orderID", drv["OrderID"].ToString());
                        chk.Attributes.Add("onclick",
                                       string.Format(showOrderDetails,
                                                     drv["OrderID"], drv["NoPallets"], drv["OrderGroupID"],
                                                     drv["OrderGroupGroupedPlanning"].ToString().ToLower(), drv["PalletSpaces"],
                                                     drv["Weight"].ToString(), collectionPoint, deliveryPoint,
                                                     drv["DropOrder"] == DBNull.Value ? "0" : drv["DropOrder"].ToString(),
                                                     drv["BusinessTypeID"].ToString(), drv["BusinessType"].ToString().Trim(),
                                                     customer, deliveryAddressLines, (drv["Latitude"] == DBNull.Value) ? "0" : drv["Latitude"], (drv["Longitude"] == DBNull.Value) ? "0" : drv["Longitude"],
                                                     lblDeliverAt.Text));
                    }
                    else
                        chk.Enabled = false;

                    //chk.Attributes.Add("onclick",
                    //                   string.Format("javascript:ChangeList(event, this, {0}, {1}, {2}, {3}, {4}, {5});",
                    //                                 drv["OrderID"], drv["NoPallets"], drv["OrderGroupID"],
                    //                                 drv["OrderGroupGroupedPlanning"].ToString().ToLower(), drv["PalletSpaces"], drv["Weight"].ToString()));

                }
                PlaceHolder phPostCode = e.Item.FindControl("phPostCode") as PlaceHolder;
                if (Globals.Configuration.GPSRealtime)
                {
                    if (drv["Latitude"] != DBNull.Value && double.Parse(drv["Latitude"].ToString()) != 0)
                    {
                        HyperLink hl = new HyperLink();
                        hl.Text = drv["PostCode"].ToString();
                        hl.NavigateUrl = string.Format("javascript:DeliverPointAlterPosition({0});", drv["DeliveryPointId"].ToString());
                        phPostCode.Controls.Add(hl);
                    }
                    else
                        phPostCode.Controls.Add(new LiteralControl(drv["PostCode"].ToString()));
                }
                else
                    phPostCode.Controls.Add(new LiteralControl(drv["PostCode"].ToString()));

                if (orders.Count > 0 && orders.Contains(int.Parse(chk.Attributes["OrderID"].ToString())) && chk != null)
                {
                    chk.Checked = true;
                    e.Item.Selected = true;
                    NoPallets += (int)drv["NoPallets"];
                }

                //if (imgOrderCollectionDeliveryNotes != null)
                //{
                //    // if it is the orders 1st collection or orders delivery destination - show the collection / delivery note.
                //    if (!string.IsNullOrEmpty(drv["DeliveryNotes"].ToString()))
                //    {
                //        imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseover", "javascript:ShowOrderCollectionDeliveryNotes(this," + drv["OrderID"].ToString() + ",'" + bool.FalseString + "');");
                //        imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseout", "javascript:hideAd();");
                //        imgOrderCollectionDeliveryNotes.Attributes.Add("class", "orchestratorLink");
                //    }
                //    else
                //        imgOrderCollectionDeliveryNotes.Visible = false;
                //}

                //if the order has been booked in show the tick
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.BookedIn && drv["BookedInDateTime"] != DBNull.Value)
                {
                    imgOrderBookedIn.Visible = true;
                    imgOrderBookedIn.Alt = string.Format("Booked In by {0} at {1} with {2}; references: {3}", drv["BookedInByUserName"].ToString(), ((DateTime)drv["BookedInDateTime"]).ToString("dd/MM/yy HH:mm"), drv["BookedInWith"].ToString(), drv["BookedInReferences"].ToString());
                }

                //TODO: Goods Type Images need to be controlled better than this
                //      we need to have system defined goods types as per Extra Types
                if (drv["IsHazardous"] != DBNull.Value && (int)drv["IsHazardous"] == 1)
                {
                    HtmlImage imgHazard = e.Item.FindControl("imgGoodsType") as HtmlImage;
                    imgHazard.Visible = true;
                }

                //if the order requires booking show the requires book in icon instead and hook up the method call
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.Required)
                {
                    imgOrderBookedIn.Src = "/images/star.gif";
                    imgOrderBookedIn.Attributes.Add("onclick", "ShowBookIn(this," + drv["OrderID"].ToString() + "," + ((DateTime)drv["DeliveryFromDateTime"]).Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + "," + ((DateTime)drv["DeliveryDateTime"]).Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + ");");
                    imgOrderBookedIn.Alt = "Please click to mark as booked in.";
                    imgOrderBookedIn.Visible = true;
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

                string deliveryDriver = drv.Row.Field<string>("DeliveryDriver");

                if (!string.IsNullOrEmpty(deliveryDriver))
                {
                    var plcDeliveryDriver = (PlaceHolder)e.Item.FindControl("plcDeliveryDriver");
                    int resourceManifestID = drv.Row.Field<int>("ResourceManifestID");

                    if (resourceManifestID > 0)
                    {
                        plcDeliveryDriver.Controls.Add(new HyperLink
                        {
                            NavigateUrl = string.Format("javascript:showManifest({0});", resourceManifestID),
                            Text = deliveryDriver,
                        });
                    }
                    else
                    {
                        plcDeliveryDriver.Controls.Add(new Label { Text = deliveryDriver });
                    }
                }

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

        void grdDeliveries_ItemCommand(object source, GridCommandEventArgs e)
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
                    grdDeliveries.Rebind();
                }
            }

            if (e.CommandName.ToLower() == "showall")
            {
                this.RowFilter = string.Empty;
                grdDeliveries.Rebind();
            }

            if (e.CommandName.ToLower() == "sort")
                _updateGridSettings = true;
        }

        void grdDeliveries_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            List<int> trafficAreaIDs = new List<int>();
            foreach (ListItem li in cblTrafficAreas.Items)
                if (li.Selected)
                    trafficAreaIDs.Add(int.Parse(li.Value));

            Facade.IOrder facOrder = new Facade.Order();
            DateTime endDate = dteEndDate.SelectedDate.Value;

            int clientId = 0;
            int.TryParse(cboClient.SelectedValue,out clientId);

            int collectionPointId = ucCollectionPointFilter.PointID;

            var organisationReferenceFieldNames = string.Join(",", from li in this.cblOrganisationReferenceColumns.Items.Cast<ListItem>() where li.Selected select li.Text);

            DataSet orderData = facOrder.GetForDelivery(dteStartDate.SelectedDate.Value, endDate, trafficAreaIDs, businessTypeCheckList.SelectedBusinessTypeIDs, this.SurchargeIDs, this.ServiceLevelIDs, rblDateFiltering.SelectedItem == null ? C_DEFAULT_DATE_FILTERING : (eDateFilterType)Enum.Parse(typeof(eDateFilterType), rblDateFiltering.SelectedItem.Value), chkRequiresBookingIn.Checked, chkBookedIn.Checked, chkNoTrunkDate.Checked, chkShowNotPlanned.Checked, chkShowAll.Checked, chkDoesNotRequireBookingIn.Checked, this.chkOrderOnlyShowRefusals.Checked, this.chkOffTrailer.Checked, clientId, collectionPointId, organisationReferenceFieldNames);
            DataView dv = orderData.Tables[0].DefaultView;

            if (!string.IsNullOrEmpty(RowFilter))
                dv.RowFilter = this.RowFilter;

            grdDeliveries.DataSource = dv;
            grdDeliveries.MasterTableView.GroupsDefaultExpanded = !chkStartCollapsed.Checked;
        }

        #endregion Grid View Events

        #endregion Event Handlers

        #region Grid Saving

        void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            SaveGridSettings(true);
            this.btnSaveGridSettings.Text = "Update Grid Layout";
        }

        private void SaveGridSettings(bool refreshGrid)
        {
            // get any columns that need to be hidden
            foreach (ListItem li in this.cblGridColumns.Items)
            {
                this.grdDeliveries.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            foreach (ListItem li in this.cblOrganisationReferenceColumns.Items)
            {
                this.grdDeliveries.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            Utilities.SaveGridSettings(this.grdDeliveries, eGrid.DeliveriesNew, Page.User.Identity.Name);

            // forces the grid to pick up the column settings if changed
            if (refreshGrid)
            {
                LoadGridSettings();
                grdDeliveries.Rebind();
            }
        }

        public void ResetGridLayout()
        {
            Utilities.ResetGridSettings(eGrid.DeliveriesNew, Page.User.Identity.Name);
        }

        private void LoadGridSettings()
        {
            IEnumerable<string> columnsToHide;
            Utilities.LoadSettings(this.grdDeliveries, eGrid.DeliveriesNew, out columnsToHide, Page.User.Identity.Name);

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

        #region Page Methods for Job Creation and Manifest Production

        [System.Web.Services.WebMethod]
        public static string CreateDeliveryJob(
            List<int> orderIDs, int businessTypeID, int collectionPointID, DateTime? trunkCollectionDate, DateTime? trunkCollectionTime, 
            int driverResourceID, int vehicleResourceID, int trailerResourceID, int planningCatgeoryID, string userName, 
            bool createManifest, string resourceName, int? subcontractorID, int? subcontractType, decimal? subcontractRate, bool showAsCommunicated, 
            string manifestTitle, DateTime? manifestDate, bool printPods, bool printPils, DateTime? TrunkDate, bool createOrderGroup, 
            int? allocatedToIdentityID, string loadNo)
        {
            if (subcontractorID.HasValue && 
                subcontractorID == Globals.Configuration.PalletNetworkID && 
                subcontractType.HasValue && 
                ((eSubContractorDataItem)subcontractType.Value) == eSubContractorDataItem.Job)
            {
                subcontractType = (int)eSubContractorDataItem.Order;
            }

            string retVal = "";

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
            dtOrders.Columns.Add("DeliveryOrder", typeof(int));
            dtOrders.Columns.Add("CollectFromPointID", typeof(int));
            dtOrders.Columns.Add("CollectAtDateTime", typeof(DateTime));
            dtOrders.Columns.Add("CollectAtAnyTime", typeof(bool));

            DataRow row = null;
            for (int i = 0; i < orderIDs.Count; i++)
            {
                // we are using the ordinal position of the orderid in the list to denote the delivery order
                row = dtOrders.Select("OrderID =" + orderIDs[i])[0];
                row["DeliveryOrder"] = i;
                row.AcceptChanges();
            }

            // If we are planning the deliveries before the collection and are planning to pick these up from a cross dock location
            // set the relevant information
            if (collectionPointID > 0)
            {
                DateTime collectionDateTime = trunkCollectionDate.Value;
                if (trunkCollectionTime.HasValue)
                    collectionDateTime = collectionDateTime.Add(trunkCollectionTime.Value.TimeOfDay);

                foreach (DataRow orderRow in dtOrders.Rows)
                {
                    orderRow["CollectFromPointID"] = collectionPointID;
                    orderRow["CollectAtDateTime"] = collectionDateTime;
                    orderRow["CollectAtAnyTime"] = !trunkCollectionTime.HasValue;
                    orderRow.AcceptChanges();
                }
            }
            else
            {
                foreach (DataRow orderRow in dtOrders.Rows)
                {
                    orderRow["CollectFromPointID"] = orderRow["CollectionRunDeliveryPointID"];
                    orderRow["CollectAtDateTime"] = orderRow["CollectionRunDeliveryDatetime"];
                    orderRow["CollectAtAnyTime"] = orderRow["CollectionRunDeliveryIsAnytime"];
                    orderRow.AcceptChanges();
                }
            }
            #endregion
            // 

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

                    int orderGroupId = -1;
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
                            orderGroupId = facOrderGroup.Create(addOrders[0], true, string.Empty, string.Empty, userName);
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

                    #endregion Create Order Group

                    if (!String.IsNullOrEmpty(loadNo))
                    {
                        //update order customer order numbers
                        foreach (DataRow orderRow in dtOrders.Rows)
                        {
                            orderRow["CustomerOrderNumber"] = loadNo;
                            orderRow.AcceptChanges();
                            facorder.UpdateCustomerOrderNumber((int)orderRow["OrderID"], loadNo, userName);
                        }
                    }

                    Facade.IJob facJob = new Facade.Job();
                    Entities.FacadeResult res = facJob.CreateJobForDeliveries(businessTypeID, OrderData, userName);

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

                    if (allocatedToIdentityID.HasValue && allocatedToIdentityID.Value > 0)
                    {
                        var ordersToAllocate = dtOrders.AsEnumerable().Select(dr => dr.Field<int>("OrderID"));
                        SaveAllocation(allocatedToIdentityID.Value, ordersToAllocate, userName);
                    }

                    if (subcontractorID.HasValue && subcontractorID.Value > 0)
                    {
                        DataAccess.IBusinessType dacBusinessType = new DataAccess.BusinessType();
                        DataSet _dsBusinessType = dacBusinessType.GetAll();
                        _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
                        DataRow drBusinessType = _dsBusinessType.Tables[0].Rows.Find(businessTypeID);
                        #region Sub contract the job

                        eSubContractorDataItem subOutChoice = eSubContractorDataItem.Job;
                        if (subcontractType.HasValue)
                            subOutChoice = (eSubContractorDataItem)subcontractType.Value;

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
                            res = facJobSubContractor.Create(jobID, new List<int>(), orderIDs, jobSubContractor, DateTime.Now, userName, true);
                        else
                            res = facJobSubContractor.Create(jobID, new List<int>(), new List<int>(), jobSubContractor, DateTime.Now, userName, true);

                        // Create theDriver Manifest for the job...
                        if (res.Success && createManifest)
                        {
                            Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
                            Entities.ResourceManifestJob rmj = null;

                            rm.ManifestDate = manifestDate ?? DateTime.Today;

                            if (!string.IsNullOrEmpty(manifestTitle))
                                rm.Description = manifestTitle;
                            else
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

                        if (TrunkDate.HasValue)
                        {
                            // Assign the Trunk date to the selected Orders
                            DataAccess.Order DAL = new Orchestrator.DataAccess.Order();
                            foreach (int orderID in orderIDs)
                                DAL.UpdateTrunkDate(orderID, TrunkDate.Value.ToLocalTime(), userName);

                        }
                        #endregion
                    }
                    else
                    {
                        #region resource this to a driver if supplied and create a drive manifest
                        if (planningCatgeoryID > 0 || driverResourceID > 0 || vehicleResourceID > 0 || trailerResourceID > 0)
                        {
                            if (planningCatgeoryID > 0)
                                res = facInstruction.AssignPlanningCategory(instructionIDs, jobID, planningCatgeoryID, DateTime.Now, userName);
                            else
                                res = facInstruction.PlanInstruction(instructionIDs, jobID, driverResourceID, vehicleResourceID, trailerResourceID, DateTime.Now, userName);

                            // Create theDriver Manifest for the job only if this has had a driver assigned to it
                            if (driverResourceID > 0)
                            {
                                if (res.Success && createManifest)
                                {
                                    Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
                                    Entities.ResourceManifestJob rmj = null;

                                    rm.ManifestDate = manifestDate ?? DateTime.Today;

                                    if (!string.IsNullOrEmpty(manifestTitle))
                                        rm.Description = manifestTitle;
                                    else
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
                        }
                        #endregion
                    }


                    #endregion

                    if (printPods & Globals.Configuration.PodLabelsEnabled)
                    {
                        // Print the POD labels 
                        Facade.PODLabelPrintingService podLabelPrintingService = new Facade.PODLabelPrintingService();
                        bool isPrinted = podLabelPrintingService.PrintPODLabels(orderIDs);
                    }
                    tran.Complete();
                }
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                string sensibleErrorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    sensibleErrorMessage += "\n" + ex.Message;
                }

                throw new ApplicationException(sensibleErrorMessage);
            }

            return retVal;
        }

        [System.Web.Services.WebMethod]
        public static string GenerateAndShowPil(int JobId)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Facade.IOrder facOrder = new Facade.Order();
            var orders = facOrder.GetForJob(JobId);

            string orderIds = string.Empty;
            foreach (DataRow order in orders.Tables[0].Rows)
            {
                orderIds += order["OrderID"] + ",";
            }
            orderIds = orderIds.TrimEnd(',');

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
                
            eReportType reportType = eReportType.PIL;
            dsPIL = facLoadOrder.GetPILData(orderIds);
            DataView dvPIL;

            if ((bool)dsPIL.Tables[0].Rows[0]["IsPalletNetwork"])
            {
                reportType = Globals.Configuration.PalletNetworkLabelID;

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
            Hashtable htReportInformation = new Hashtable(10);
            htReportInformation[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = reportType;
            htReportInformation[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
            htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dvPIL;
            htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            htReportInformation[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            Guid guid = Guid.NewGuid();
            HttpContext.Current.Session.Add(guid.ToString(), htReportInformation);
            return guid.ToString();
        }

        private static void SaveAllocation(int allocatedToID, IEnumerable<int> ordersToAllocate, string userName)
        {
            Facade.IOrder facOrder = new Facade.Order();
            foreach (int orderId in ordersToAllocate)
            {
                facOrder.UpdateAllocation(orderId, allocatedToID, true, userName);
            }
        }

        private static bool CommunicateInstructions(int jobID, int driverID, int vehicleID, string userId)
        {
            Entities.DriverCommunication communication = new Entities.DriverCommunication();
            communication.Comments = "Communicated via Deliveries Screen";
            communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;

            string mobileNumber = "unknown";
            communication.NumberUsed = mobileNumber;

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

            Hashtable htReportInformation = new Hashtable(10);
            htReportInformation.Add(Orchestrator.Globals.Constants.ReportTypeSessionVariable, eReportType.LoadingSummarySheet);
            htReportInformation.Add(Orchestrator.Globals.Constants.ReportParamsSessionVariable, reportParams);
            htReportInformation.Add(Orchestrator.Globals.Constants.ReportDataSessionTableVariable, dsLSS);
            htReportInformation.Add(Orchestrator.Globals.Constants.ReportDataSessionSortVariable, String.Empty);
            htReportInformation.Add(Orchestrator.Globals.Constants.ReportDataMemberSessionVariable, "Table");

            Guid guid = Guid.NewGuid();
            HttpContext.Current.Session.Add(guid.ToString(), htReportInformation);
            return guid.ToString();
        }

        #endregion

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

                // Changed to completed from booked/planned as the jobs are now created as communicated.
                if (i.InstructionTypeId == (int)eInstructionType.Load && (i.InstructionState != eInstructionState.Completed))
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

        [System.Web.Services.WebMethod]
        public static string UpdateJob(int jobID, List<int> orderIDs, int collectionPointID, DateTime? trunkCollectionDate, DateTime? trunkCollectionTime, int? instructionID, string userName, int userIdentityID, bool updateExistingManifest, DateTime? manifestDate, string manifestTitle, string resourceName)
        {
            string retval = string.Empty;
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
                dtOrders.Columns.Add("DeliveryOrder", typeof(int));
                dtOrders.Columns.Add("CollectFromPointID", typeof(int));
                dtOrders.Columns.Add("CollectAtDateTime", typeof(DateTime));
                dtOrders.Columns.Add("CollectAtAnyTime", typeof(bool));

                DataRow r = null;
                for (int i = 0; i < orderIDs.Count; i++)
                {
                    // we are using the ordinal position of the orderid in the list to denote the delivery order
                    r = dtOrders.Select("OrderID =" + orderIDs[i])[0];
                    r["DeliveryOrder"] = i;
                    r.AcceptChanges();
                }

                // If we are planning the deliveries before the collection and are planning to pick these up from a cross dock location
                // set the relevant information
                if (collectionPointID > 0)
                {
                    DateTime collectionDateTime = trunkCollectionDate.Value;
                    if (trunkCollectionTime.HasValue)
                        collectionDateTime = collectionDateTime.Add(trunkCollectionTime.Value.TimeOfDay);

                    foreach (DataRow orderRow in dtOrders.Rows)
                    {
                        orderRow["CollectFromPointID"] = collectionPointID;
                        orderRow["CollectAtDateTime"] = collectionDateTime;
                        orderRow["CollectAtAnyTime"] = !trunkCollectionTime.HasValue;
                        orderRow.AcceptChanges();
                    }
                }
                else
                {
                    foreach (DataRow orderRow in dtOrders.Rows)
                    {
                        orderRow["CollectFromPointID"] = orderRow["CollectionPointID"];
                        orderRow["CollectAtDateTime"] = orderRow["CollectionDateTime"];
                        orderRow["CollectAtAnyTime"] = orderRow["CollectionIsAnytime"];
                        orderRow.AcceptChanges();
                    }
                }

                #endregion

                #region Update the Job

                Facade.IJob facJob = new Facade.Job();
                Entities.Job job = facJob.GetJob(jobID, true);
                Entities.FacadeResult facResult = null;

                Entities.InstructionCollection collections = job.Instructions.GetForInstructionType(eInstructionType.Load);
                List<Entities.Instruction> amendedCollections = new List<Orchestrator.Entities.Instruction>();
                Entities.Instruction iCollect = null;
                Entities.CollectDrop cd = null;

                Entities.InstructionCollection drops = job.Instructions.GetForInstructionType(eInstructionType.Drop);
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
                    int pointID = (int)row["CollectFromPointID"];
                    DateTime bookedDateTime = (DateTime)row["CollectAtDateTime"];
                    bool collectionIsAnytime = (bool)row["CollectAtAnyTime"];

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
                            iCollect = collections.GetForInstructionTypeAndPoint(eInstructionType.Load, pointID);
                        }

                    if (iCollect == null)
                    {
                        iCollect = new Orchestrator.Entities.Instruction();
                        iCollect.InstructionTypeId = (int)eInstructionType.Load;
                        iCollect.BookedDateTime = bookedDateTime;
                        if ((bool)row["CollectAtAnytime"])
                            iCollect.IsAnyTime = true;
                        point = facPoint.GetPointForPointId(pointID);
                        iCollect.PointID = pointID;
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
                dvDrops.Sort = "DeliveryOrder ASC";
                DataTable dtDrops = dvDrops.ToTable();
                foreach (DataRow row in dtDrops.Rows)
                {
                    #region Deliveries
                    bool newdelivery = false;
                    newdelivery = false;
                    int deliveryPointID = (int)row["DeliveryPointID"];
                    DateTime deliveryDateTime = (DateTime)row["DeliveryDateTime"];
                    bool deliveryIsAnyTime = (bool)row["DeliveryIsAnyTime"];

                    // if this setting is true then we want to create a new instruction for the order so set the instruction to null.
                    if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                        iDrop = null;
                    else
                        iDrop = drops.GetForInstructionTypeAndPoint(eInstructionType.Drop, deliveryPointID);

                    if (iDrop == null)
                    {
                        iDrop = new Orchestrator.Entities.Instruction();
                        iDrop.InstructionTypeId = (int)eInstructionType.Drop;
                        iDrop.BookedDateTime = deliveryDateTime;
                        if ((bool)row["DeliveryIsAnytime"])
                            iDrop.IsAnyTime = true;
                        point = facPoint.GetPointForPointId(deliveryPointID);
                        iDrop.ClientsCustomerIdentityID = point.IdentityId;
                        iDrop.PointID = deliveryPointID;
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

                foreach (int i in orderIDs)
                {
                    Entities.Order order = facOrder.GetForOrderID(i);
                    if (facResult.Success)
                        foreach (var exportJob in Orchestrator.Application.GetSpecificImplementations<Application.IExportJob>())
                            exportJob.AddOrder(job, order, userName);
                }

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
                                rm.ManifestDate = manifestDate.Value;

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

        #endregion

        #region Page methods for Booking In

        [System.Web.Services.WebMethod]
        public static bool BookIn(int orderID, string bookedInBy, string bookedInWith, string bookedInReferences, string dateOption, string datefromDate, string dateFromTime, string dateFromByDate, string dateFromByTime)
        {
            Orchestrator.EF.DataContext context = new Orchestrator.EF.DataContext();
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);

            Facade.IOrder facOrder = new Facade.Order();
            Entities.Order orderOriginal = facOrder.GetForOrderID(orderID);
            
            order.BookedIn = true;
            order.BookedInByUserName = bookedInBy;
            order.BookedInDateTime = DateTime.Now;
            order.BookedInStateId = (int)eBookedInState.BookedIn;
            order.BookedInWith = bookedInWith;
            order.BookedInReferences = bookedInReferences;

            // Set the date and time for the Delivery based on the book in etails
            if (int.Parse(dateOption) == 0) //window
            {
                order.DeliveryFromDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
                order.DeliveryDateTime = DateTime.Parse(dateFromByDate).Add(TimeSpan.Parse(dateFromByTime));
            }
            else
            {
                order.DeliveryFromDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
                order.DeliveryDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
            }

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = bookedInBy;
            context.SaveChanges(true);

            Entities.Order orderNew = facOrder.GetForOrderID(orderID);

            var exports = Orchestrator.Application.GetSpecificImplementations<Application.IExportOrder>();

            if (exports.Any())
            {
                foreach (var export in exports)
                {
                    export.Update(orderNew, orderOriginal, bookedInBy);
                }
            }

            return true;
        }

        #endregion

        #region Page Method for Changing order business types

        [System.Web.Services.WebMethod]
        public static bool UpdateBusinessType(string orderIDCSV, int businessTypeID, string userName)
        {
            Facade.IOrder facOrder = new Facade.Order();
            return facOrder.UpdateOrdersBusinessType(orderIDCSV, businessTypeID, userName);
        }

        #endregion

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
            public decimal PalletSpaces;

            public string DeliveryCustomer;
            public string DeliveryPoint;
            public string CollectionPoint;
            public int OrderStatusID;
            public decimal Latitude;
            public decimal Longitude;

            public int OrderGroupID;
            public bool OrderGroupGroupedPlanning;
            public int BusinessTypeID;
            public string BusinessType;

            public miniOrder(int orderID, int customerIdentityID, string customer, string customerOrderNumber, string deliveryOrderNumber, int noPallets, decimal palletSpaces, decimal weight)
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
                PalletSpaces = palletSpaces;
                Longitude = 0.0M;
                Latitude = 0.0M;
                OrderGroupID = -1;
                OrderGroupGroupedPlanning = false;
                BusinessType = string.Empty;
                BusinessTypeID = -1;
            }
        }

    }

}
