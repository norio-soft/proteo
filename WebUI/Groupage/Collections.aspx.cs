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
using Orchestrator.BusinessRules;
using Telerik.Web.UI;
using System.Xml.Serialization;

namespace Orchestrator.WebUI.Groupage
{
    public partial class Collections : Orchestrator.Base.BasePage
    {

        #region Constants

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";
        private const string C_StartDate_VS = "C_StartDate_VS";
        private const string C_EndDate_VS = "C_EndDate_VS";

        private const string C_GRID_NAME = "__collectionsGrid";
        private string _orders = string.Empty;
        private const string C_ONLYSHOWNONCOLLECTED = "_collectionsOnlyShowNonCollected";

        #endregion

        #region Public Properies

        private DateTime StartDate
        {
            get { return (DateTime)ViewState[C_StartDate_VS]; }
            set { ViewState[C_StartDate_VS] = value; }
        }

        private DateTime EndDate
        {
            get { return (DateTime)ViewState[C_EndDate_VS]; }
            set { ViewState[C_EndDate_VS] = value; }
        }

        private string RowFilter
        {
            get {return (string)this.ViewState["_rowFilter"];}
            set { this.ViewState["_rowFilter"] = value; }
        }


        protected int NoPallets
        {
            get { return this.ViewState["_noPalletsSelected"] == null ? 0 : (int)this.ViewState["_noPalletsSelected"]; }
            set { this.ViewState["_noPalletsSelected"] = value; }

        }

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
                        orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                        if (_orders.Length > 0)
                            _orders += ",";
                        _orders += orderID.ToString();
                    }
                }

                return _orders;
            }
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

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);

            if (!IsPostBack)
            {
                btnCreateCollection.PostBackUrl = "collectionjob.aspx";
                //TODO: Replace this on Collection and Deliveries
                //dteStartDate.MinValue = DateTime.Today;                            
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

                if (Session[C_ONLYSHOWNONCOLLECTED] != null && Session[C_ONLYSHOWNONCOLLECTED] is bool)
                    chkShowUnPlannedOnly.Checked = (bool)Session[C_ONLYSHOWNONCOLLECTED];
                
                // Get the Dates from the filter if one exists
                GetDates();   
                
            }

        }

        private void GetDates()
        {

            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (_filter != null)
            {
                if (_filter.FilterStartDate == DateTime.MinValue)
                {
                    dteStartDate.SelectedDate = DateTime.Today.Subtract(DateTime.Now.TimeOfDay);
                    dteEndDate.SelectedDate = DateTime.Today.AddDays(1);
                }
                else
                {
                    dteStartDate.SelectedDate = _filter.FilterStartDate;
                    dteEndDate.SelectedDate = _filter.FilterEnddate;
                }

                cblTrafficAreas.ClearSelection();
                if (_filter.TrafficAreaIDs.Any())
                {
                    foreach (ListItem li in cblTrafficAreas.Items)
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
                _filter.FilterStartDate = DateTime.Today.Subtract(DateTime.Today.TimeOfDay);
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

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);
            this.grdOrders.ItemDataBound +=new GridItemEventHandler(grdOrders_ItemDataBound);
        }

        #endregion

        #region Private Methods

        private void CaptureOrderIDs()
        {
            List<int> orders = new List<int>();
            foreach (GridDataItem gdi in this.grdOrders.Items)
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
            Entities.TrafficSheetFilter _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            _filter.FilterStartDate = dteStartDate.SelectedDate.Value.Subtract(dteStartDate.SelectedDate.Value.TimeOfDay);
            _filter.FilterEnddate = dteEndDate.SelectedDate.Value;
            _filter.BusinessTypes.Clear();

            foreach (ListItem li in cblBusinessTypes.Items)
                if (li.Selected)
                    _filter.BusinessTypes.Add(int.Parse(li.Value));

            _filter.TrafficAreaIDs.Clear();

            Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
            foreach (ListItem li in cblTrafficAreas.Items)
            {
                if (li.Selected)
                    _filter.TrafficAreaIDs.Add(int.Parse(li.Value));
            }

            SetCookie(_filter);

            // Persist the checkbox selection value to session.
            Session[C_ONLYSHOWNONCOLLECTED] = chkShowUnPlannedOnly.Checked;

            grdOrders.Rebind();
        }

        #endregion

        #region Grid View Events
        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                CheckBox chk = e.Item.FindControl("chkOrderID") as CheckBox;
                HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces");
                HtmlImage imgOrderCollectionDeliveryNotes = (HtmlImage)e.Item.FindControl("imgOrderCollectionDeliveryNotes");

                List<int> orders = new List<int>();
                if (Session[C_GRID_NAME] != null)
                    orders = (List<int>)Session[C_GRID_NAME];

                DataRowView drv = (DataRowView)e.Item.DataItem;
                if ((bool)drv["PlannedForCollection"])
                    e.Item.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDFBF");

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
                                       string.Format("javascript:ChangeList(event, this, {0}, {1}, {2}, {3});",
                                                     drv["OrderID"], drv["NoPallets"], drv["OrderGroupID"],
                                                     drv["OrderGroupGroupedPlanning"].ToString().ToLower()));

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

                if (imgOrderCollectionDeliveryNotes != null)
                {
                    // if it is the orders 1st collection or orders delivery destination - show the collection / delivery note.
                    if (!string.IsNullOrEmpty(drv["CollectionNotes"].ToString()))
                    {
                        imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseover", "javascript:ShowOrderCollectionDeliveryNotes(this," + drv["OrderID"].ToString() + ",'" + bool.TrueString + "');");
                        imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseout", "javascript:closeToolTip();");
                        imgOrderCollectionDeliveryNotes.Attributes.Add("class", "orchestratorLink");
                    }
                    else
                        imgOrderCollectionDeliveryNotes.Visible = false;
                }
            }
            else if (e.Item is GridGroupHeaderItem)
            {
                GridGroupHeaderItem item = (GridGroupHeaderItem)e.Item;
                DataRowView groupDataRow = (DataRowView)e.Item.DataItem;
                item.DataCell.Text = string.Format("{0} ({1})", groupDataRow["TrafficAreaShortName"], groupDataRow["Count"]);
            }
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();

            DateTime endDate = dteEndDate.SelectedDate.Value.Add(new TimeSpan(23, 59, 59));
            DataSet orderData = facOrder.GetOrdersForCollection(dteStartDate.SelectedDate.Value , endDate, TrafficAreaIDs, BusinessTypeIDs);
            bool allowJobCreation = false;
            string filter = this.RowFilter;
            DataView dv = orderData.Tables[0].DefaultView;
            if (chkShowCrossDockedOnly.Checked && !chkShowUnPlannedOnly.Checked)
            {
                if (!string.IsNullOrEmpty(filter) && filter.IndexOf("LastPlannedOrderAction") <0)
                {
                    filter = filter + " LastPlannedOrderAction = 'Cross Dock'";
                }
                else
                {
                    filter = " LastPlannedOrderAction = 'Cross Dock'";
                }

            }else if (chkShowUnPlannedOnly.Checked)
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += "PlannedForCollection = 0";
                else
                    filter = "PlannedForCollection = 0";
            }

            if (!string.IsNullOrEmpty(filter))
                dv.RowFilter = filter;

            grdOrders.DataSource = dv;    
            allowJobCreation = orderData.Tables[0].Rows.Count > 0;
            

            btnCreateCollection.Visible = allowJobCreation;
            divCreateCollectionButtonBar.Visible = allowJobCreation;

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
                    grdOrders.Rebind();
                }
            }

            if (e.CommandName.ToLower() == "showall")
            {
                this.RowFilter = string.Empty;
                grdOrders.Rebind();
            }
        }

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }
        #endregion

        #endregion
    }
}
