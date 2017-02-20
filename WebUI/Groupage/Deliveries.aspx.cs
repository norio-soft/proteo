using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
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

namespace Orchestrator.WebUI.Groupage
{
    public partial class Deliveries : Orchestrator.Base.BasePage
    {
        #region Constants

        private bool _exportOrderEnabled = false;
        private int _noPallets = 0;
        private decimal _noPalletSpaces = 0;
        private decimal _weight = 0.0M;

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";
        private string _orders = string.Empty;
        private List<int> _ordersList;
        private readonly string VS_ROWFILTER = "_rowFilter";
        private const string C_GRID_NAME = "__deliveriesGrid";
        private const string C_SELECTED_SERVICE_LEVELS = "SelectedServiceLevels";
        private const string C_SELECTED_SURCHARGES = "SelectedSurcharges";
        private const eDateFilterType C_DEFAULT_DATE_FILTERING = eDateFilterType.Delivery_Date;

        #endregion

        #region properties

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

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
                        orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
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
                        orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                        if (_orders.Length > 0)
                            _orders += ",";
                        _orders += orderID.ToString();
                    }
                }

                return _orders;
            }
        }

        protected int NoPallets
        {
            get
            {
                _noPallets = 0;
                int noOfPalletsForRow = 0;

                foreach (GridItem row in grdDeliveries.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {

                        noOfPalletsForRow = int.Parse(row.Cells[grdDeliveries.MasterTableView.Columns.FindByUniqueName("NoPallets").OrderIndex].Text);
                        _noPallets += noOfPalletsForRow;
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

        public IEnumerable<int> BusinessTypeIDs
        {
            get
            {
                List<int> businessTypeIDs = new List<int>();
                int holding = 0;
                foreach (ListItem li in cblBusinessTypes.Items)
                    if (li.Selected && int.TryParse(li.Value, out holding) && !businessTypeIDs.Contains(holding))
                        businessTypeIDs.Add(holding);

                return businessTypeIDs;
            }
        }

        public IEnumerable<int> ServiceLevelIDs
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

        public IEnumerable<int> SurchargeIDs
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

        public IEnumerable<int> TrafficAreaIDs
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

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            _exportOrderEnabled = bool.Parse(ConfigurationManager.AppSettings["ExportOrderEnabled"].ToLower());

            if (!IsPostBack)
            {
                // Show/hide the "Export to Palletforce" button depending on whether a module id is present in the config.
                btnExportOrder.Visible = _exportOrderEnabled;

                btnCreateDelivery.PostBackUrl = "DeliveryJob.aspx?" + this.CookieSessionID;

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

                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                DataSet dsBusinessTypes = facBusinessType.GetAll();

                cblBusinessTypes.DataSource = dsBusinessTypes;
                cblBusinessTypes.DataBind();

                foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                {
                    RadMenuItem rmi = new RadMenuItem();
                    rmi.Text = row["Description"].ToString();
                    rmi.Value = row["BusinessTypeID"].ToString();
                    RadMenu1.Items.Add(rmi);
                }

                Facade.IOrderServiceLevel facOrderServiceLevel = new Facade.Order();
                cblServiceLevel.DataSource = facOrderServiceLevel.GetAll();
                cblServiceLevel.DataBind();

                Facade.ExtraType facExtraType = new Facade.ExtraType();
                cblSurcharges.DataSource = facExtraType.GetAllForFiltering();
                cblSurcharges.DataBind();

                PreSelectItems(cblServiceLevel, C_SELECTED_SERVICE_LEVELS);
                PreSelectItems(cblSurcharges, C_SELECTED_SURCHARGES);

                GetDates();
            }
        }

        private void PreSelectItems(CheckBoxList cbl, string sessionKey)
        {
            if (Session[sessionKey] != null && Session[sessionKey] is IEnumerable<int>)
            {
                foreach (int id in (IEnumerable<int>)Session[sessionKey])
                {
                    ListItem item = cbl.Items.FindByValue(id.ToString());
                    if (item != null)
                        item.Selected = true;
                }
            }
        }

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

                foreach (ListItem li in cblTrafficAreas.Items)
                {
                    if (_filter.TrafficAreaIDs.Exists(i => i.ToString().Equals(li.Value)))
                        li.Selected = true;
                }

                cblBusinessTypes.ClearSelection();
                if (_filter.BusinessTypes.Count > 0)
                {
                    foreach (ListItem li in cblBusinessTypes.Items)
                        if (_filter.BusinessTypes.Contains(int.Parse(li.Value)))
                            li.Selected = true;
                }
            }
            else
            {
                _filter = GetDefaultFilter();
                _filter.FilterStartDate = DateTime.Today;
                _filter.FilterEnddate = DateTime.Today.AddDays(1);
                SetCookie(_filter);

                dteStartDate.SelectedDate = DateTime.Today;
                dteEndDate.SelectedDate = DateTime.Today.AddDays(1);
                if (!string.IsNullOrEmpty(Request.QueryString["BT"]))
                {
                    cblBusinessTypes.Items.FindByValue(Request.QueryString["BT"]).Selected = true;
                }
            }

        }

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdDeliveries.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdDeliveries.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);
            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.grdDeliveries.ItemDataBound += new GridItemEventHandler(grdDeliveries_ItemDataBound);
            this.btnSaveGridSettings.Click += new EventHandler(btnSaveGridSettings_Click);
            //this.raxManager.AjaxRequest +=new RadAjaxManager.AsyncRequestDelegate(raxManager_AjaxRequest);

            LoadGridSettings();
        }

        void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            SaveGridSettings();
            this.btnSaveGridSettings.Text = "Update Grid Layout";
        }

        #region Private Methods

        private void CaptureOrderIDs()
        {
            List<int> orders = new List<int>();
            foreach (GridDataItem gdi in this.grdDeliveries.Items)
            {
                CheckBox chkOrderID = (CheckBox)gdi.FindControl("chkOrderID");
                if (chkOrderID != null && chkOrderID.Checked)
                {
                    int orderID = 0;
                    int.TryParse(gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["OrderID"].ToString(), out orderID);

                    if (orderID > 0)
                        orders.Add(orderID);
                }
            }
            Session[C_GRID_NAME] = orders;
        }

        #endregion

        #region Event Handlers

        #region Button Events

        void btnRefresh_Click(object sender, EventArgs e)
        {
            CaptureOrderIDs();
            Entities.TrafficSheetFilter _filter = GetFilter();
            _filter.FilterStartDate = dteStartDate.SelectedDate.Value;
            _filter.FilterEnddate = dteEndDate.SelectedDate.Value;
            _filter.BusinessTypes.Clear();

            foreach (int businessTypeID in BusinessTypeIDs)
                _filter.BusinessTypes.Add(businessTypeID);

            _filter.TrafficAreaIDs.Clear();

            foreach (ListItem li in cblTrafficAreas.Items)
            {
                if (li.Selected)
                    _filter.TrafficAreaIDs.Add(int.Parse(li.Value));
            }

            SetCookie(_filter);

            // Store the selected surcharge and service level ids.
            Session[C_SELECTED_SURCHARGES] = this.SurchargeIDs;
            Session[C_SELECTED_SERVICE_LEVELS] = this.ServiceLevelIDs;

            grdDeliveries.Rebind();
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
                    int orderId;
                    int.TryParse(item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString(), out orderId);

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
                if (bool.Parse(drv["IsExported"].ToString()) == true)
                {
                    e.Item.BackColor = System.Drawing.Color.Violet;
                }

                if (orders.Count > 0)
                {
                    if (orders.Contains((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"]))
                    {
                        if (chk != null)
                        {
                            chk.Checked = true;
                            e.Item.Selected = true;
                            NoPallets += (int)drv["NoPallets"];
                        }
                    }
                }

                if (chk != null)
                    chk.Attributes.Add("onclick",
                                       string.Format("javascript:ChangeList(event, this, {0}, {1}, {2}, {3}, {4}, {5});",
                                                     drv["OrderID"], drv["NoPallets"], drv["OrderGroupID"],
                                                     drv["OrderGroupGroupedPlanning"].ToString().ToLower(), drv["PalletSpaces"], drv["Weight"].ToString()));

                if (spnPalletSpaces != null)
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

                imgOrderBookedIn.Visible = Convert.ToBoolean(drv["BookedIn"].ToString());

                // Collect At column
                ITextControl lblCollectAt = (ITextControl)e.Item.FindControl("lblCollectAt");
                bool hasBeenCollected = (bool)drv["HasBeenCollected"];
                DateTime colFrom = (DateTime)drv["DeliveryRunCollectionDateTime"];
                DateTime colBy = colFrom;

                if (!hasBeenCollected)
                {
                    if (drv["CollectionByDateTime"] != DBNull.Value)
                        colBy = (DateTime)drv["CollectionByDateTime"];
                }

                if (colFrom == colBy)
                {
                    // Not timed window.
                    lblCollectAt.Text = GetDate(colFrom, false);
                }
                else
                {
                    // Timed window, may be anytime.
                    bool collectionIsAnyTime = colFrom.Date == colBy.Date && colFrom.Hour == 0 && colBy.Hour == 0 && colFrom.Millisecond == 0 && colBy.Minute == 59;
                    if (collectionIsAnyTime)
                        lblCollectAt.Text = GetDate(colFrom, true);
                    else
                        lblCollectAt.Text = GetDate(colFrom, false) + " to " + GetDate(colBy, false);
                }

                // Delivery At column
                Label lblDeliverAt = (Label)e.Item.FindControl("lblDeliverAt");
                DateTime delFrom = Convert.ToDateTime(drv["DeliveryFromDateTime"].ToString());
                DateTime delBy = Convert.ToDateTime(drv["DeliveryDateTime"].ToString());

                if (lblDeliverAt != null)
                {
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

                // Surcharges columns
                ITextControl lblSurcharge = (ITextControl)e.Item.FindControl("lblSurcharge");
                lblSurcharge.Text = string.Empty;
                int orderID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"];
                if (this.SurchargeLookupData.ContainsKey(orderID))
                    lblSurcharge.Text = this.SurchargeLookupData[orderID];


            }
            else if (e.Item is GridGroupHeaderItem)
            {
                GridGroupHeaderItem item = (GridGroupHeaderItem)e.Item;
                DataRowView groupDataRow = (DataRowView)e.Item.DataItem;
                item.DataCell.Text = string.Format("{0} ({1})", groupDataRow["TrafficAreaShortName"], groupDataRow["Count"]);
            }
        }

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
                    grdDeliveries.Rebind();

                }
            }

            if (e.CommandName.ToLower() == "showall")
            {
                this.RowFilter = string.Empty;
                grdDeliveries.Rebind();
            }

        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            List<int> businessTypeIDs = new List<int>();
            foreach (ListItem li in cblBusinessTypes.Items)
                if (li.Selected)
                    businessTypeIDs.Add(int.Parse(li.Value));

            List<int> trafficAreaIDs = new List<int>();
            foreach (ListItem li in cblTrafficAreas.Items)
                if (li.Selected)
                    trafficAreaIDs.Add(int.Parse(li.Value));

            Facade.IOrder facOrder = new Facade.Order();
            DateTime endDate = dteEndDate.SelectedDate.Value.Add(new TimeSpan(23, 59, 59));

            DataSet orderData = facOrder.GetOrdersForDelivery(dteStartDate.SelectedDate.Value, endDate, trafficAreaIDs, businessTypeIDs, this.SurchargeIDs, this.ServiceLevelIDs, rblDateFiltering.SelectedItem == null ? C_DEFAULT_DATE_FILTERING : (eDateFilterType)Enum.Parse(typeof(eDateFilterType), rblDateFiltering.SelectedItem.Value), chkBookedInYes.Checked, chkBookedInNo.Checked, null, null, chkNoTrunkDate.Checked);

            // Build the surcharge lookup data.
            this.SurchargeLookupData.Clear();
            if (orderData.Tables.Count > 1)
            {
                foreach (DataRow row in orderData.Tables[1].Rows)
                {
                    int orderID = (int)row["OrderID"];
                    string description = (string)row["Description"];

                    if (this.SurchargeLookupData.ContainsKey(orderID))
                        this.SurchargeLookupData[orderID] = string.Format("{0}, {1}", this.SurchargeLookupData[orderID], description);
                    else
                        this.SurchargeLookupData[orderID] = description;
                }
            }

            DataView dv = orderData.Tables[0].DefaultView;
            if (!string.IsNullOrEmpty(RowFilter))
            {
                dv.RowFilter = this.RowFilter;
            }

            grdDeliveries.DataSource = dv;

        }

        #endregion

        #endregion

        #region Grid Saving
        private void SaveGridSettings()
        {
            Utilities.SaveGridSettings(this.grdDeliveries, eGrid.Deliveries, Page.User.Identity.Name);
        }

        private void LoadGridSettings()
        {
            Utilities.LoadSettings(this.grdDeliveries, eGrid.Deliveries,Page.User.Identity.Name);
        }
        #endregion
    }
}
