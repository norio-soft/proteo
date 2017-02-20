using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Text;

using Orchestrator.WebUI.Security;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;

using P1TP.Components.Web.Validation;
using Telerik.Web.UI;
using System.Globalization;

namespace Orchestrator.WebUI.GoodRefused
{
	/// <summary>
	/// Summary description for List Goods Refused.
	/// </summary>

	public partial class ListGoodsRefused: Orchestrator.Base.BasePage
	{	
		#region Page Variables

        private DataSet m_ds = null;

        private string		m_refusalIdCSV      = String.Empty;
        protected Boolean   m_createJob         = false;
		protected int		m_StoreIdentityId	= 0;
		protected string	m_StoreTown			= String.Empty;
		protected int		m_StoreTownId		= 0;
		protected int		m_StorePointId		= 0;

		#endregion

        #region Public Properties

        private Entities.Point _defaultCrossDockPoint = new Entities.Point();
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

        private decimal _weight = 0.0M;
        protected decimal Weight
        {
            get
            {
                _weight = 0;
                decimal weightForRow = 0;

                foreach (GridItem row in grdRefusals.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {

                        weightForRow = decimal.Parse(row.Cells[grdRefusals.MasterTableView.Columns.FindByUniqueName("_weight").OrderIndex].Text);
                        _weight += weightForRow;
                    }
                }

                return _weight;
            }
            set { }

        }

        private int _noPallets = 0;
        protected int NoPallets
        {
            get
            {
                _noPallets = 0;
                int noOfPalletsForRow = 0;

                foreach (GridItem row in grdRefusals.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {

                        noOfPalletsForRow = int.Parse(row.Cells[grdRefusals.MasterTableView.Columns.FindByUniqueName("NoPallets").OrderIndex].Text);
                        _noPallets += noOfPalletsForRow;
                    }
                }

                return _noPallets;
            }
            set { }

        }

        private decimal _noPalletSpaces = 0;
        protected decimal NoPalletSpaces
        {
            get
            {
                _noPalletSpaces = 0;
                decimal noOfPalletSpacesForRow = 0;

                foreach (GridItem row in grdRefusals.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        decimal.TryParse(row.Cells[grdRefusals.MasterTableView.Columns.FindByUniqueName("_palletspaces").OrderIndex].Text, out noOfPalletSpacesForRow);
                        _noPalletSpaces += noOfPalletSpacesForRow;
                    }
                }

                return _noPalletSpaces;
            }
            set { }

        }
        #endregion

        #region Form Elements

        protected System.Web.UI.WebControls.RequiredFieldValidator	rfvJobStatus;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            m_createJob = Convert.ToBoolean(Request.QueryString["createJob"]);

			if (!IsPostBack)
			{
                PopulateStaticControls();

				pnlGoodsRefusedListBoxes1.Visible = pnlGoodsRefusedListBoxes2.Visible == true;
			}
		}

		protected void ListInvoice_Init(object sender, EventArgs e)
		{
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboStoreOrganisation.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboStoreOrganisation_ItemsRequested);
            this.cboStorePoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboStorePoint_ItemsRequested);

            this.btnFind.Click += new EventHandler(btnFind_Click);
            this.btnFindBottom.Click += new EventHandler(btnFind_Click);

			this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
			this.btnReportBottom.Click += new System.EventHandler(this.btnReport_Click);

            this.grdRefusals.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdRefusals_NeedDataSource);
            this.grdRefusals.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdRefusals_ItemDataBound);

            this.dlgRefusal.DialogCallBack += new EventHandler(dlgRefusal_DialogCallBack);
		}

        void dlgRefusal_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgRefusal.ReturnValue == "Refresh_Redeliveries_And_Refusals")
            {
                GetData();
            }
        }

        void grdRefusals_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                HtmlAnchor link = (HtmlAnchor)e.Item.FindControl("lnkResource");
                Label lblGoodLocation = (Label)e.Item.FindControl("lblGoodLocation");

                eGoodsRefusedLocation goodslocation = (eGoodsRefusedLocation)Enum.Parse(typeof(eGoodsRefusedLocation), lblGoodLocation.Text.Replace(" ", string.Empty));
                DateTime startOfRefusalDate = (DateTime)drv["CollectDropDateTime"];
                startOfRefusalDate = startOfRefusalDate.Subtract(startOfRefusalDate.TimeOfDay);

                eGoodsRefusedType goodsType = (eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), e.Item.Cells[6].Text.Replace(" ", ""));

                var chkInclude = (WebControl)e.Item.FindControl("chkInclude");

                HtmlAnchor hypOriginalOrder = (HtmlAnchor)e.Item.FindControl("hypOriginalOrder");
                if (drv["OriginalOrderId"] != DBNull.Value)
                {
                    int orderId = (int)drv["OriginalOrderId"];
                    string queryString = string.Format("oid={0}", orderId.ToString());
                    hypOriginalOrder.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));
                    hypOriginalOrder.InnerText = orderId.ToString();
                }

                HtmlAnchor hypNewOrder = (HtmlAnchor)e.Item.FindControl("hypNewOrder");
                if (drv["NewOrderId"] != DBNull.Value)
                {
                    int orderId = (int)drv["NewOrderId"];
                    string queryString = string.Format("oid={0}", orderId.ToString());
                    hypNewOrder.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));
                    hypNewOrder.InnerText = orderId.ToString();
                }

                if (goodsType == eGoodsRefusedType.Shorts || goodsType == eGoodsRefusedType.OverAndAccepted)
                    chkInclude.Visible = false;
                else
                {
                    //Encode any spaces etc
                    string deliveryPoint = Server.HtmlEncode(drv["ReturnLocation"].ToString());
                    string collectionPoint = Server.HtmlEncode(drv["StoreLocation"].ToString());
                    string customer = Server.HtmlEncode(drv["OrganisationName"].ToString());
                    string collectionAddressLines = Server.HtmlEncode(drv["StoreLocation"].ToString());

                    bool OnReturnRun = bool.Parse(drv["OnReturnRun"].ToString());

                    //Converts apostrophes to ASCII numerical code, will be interpreted by the browser as '
                    deliveryPoint = ConvertApostrophesToASCII(deliveryPoint);
                    collectionPoint = ConvertApostrophesToASCII(collectionPoint);
                    customer = ConvertApostrophesToASCII(customer);
                    collectionAddressLines = ConvertApostrophesToASCII(collectionAddressLines);

                    // RefusalID, Collection Point, Delivery Point, businessTypeID, businessType, deliveryCustomerName, collectionAddressLines, deliveryAddress
                    string showOrderDetails = "javascript:ChangeList(event, this, {0}, '{1}', '{2}', {3}, '{4}', '{5}', '{6}', {7});";

                    chkInclude.Visible = true;
                    chkInclude.Attributes.Add("onclick",
                                    string.Format(showOrderDetails,drv["RefusalId"], collectionPoint, deliveryPoint,drv["BusinessTypeID"] == DBNull.Value ? "-1" : drv["BusinessTypeID"].ToString(),drv["BusinessType"].ToString().Trim(), customer, collectionAddressLines, drv["DeliveryPointId"] == DBNull.Value ? -1 : drv["DeliveryPointId"]));

                    // If already on a return run, cannot be added to another return run.
                    chkInclude.Enabled = !OnReturnRun && deliveryPoint != string.Empty;
                }

                switch (goodslocation)
                {
                    case eGoodsRefusedLocation.OnTrailer:
                        // Display the text
                        if (drv["TrailerResourceId"] != DBNull.Value)
                        {
                            // Provide link to trailer's schedule.
                            try
                            {
                                Facade.ITrailer facTrailer = new Facade.Resource();
                                Entities.Trailer trailer = facTrailer.GetForTrailerId((int)drv["TrailerResourceID"]);

                                if (!trailer.IsThirdPartyTrailer)
                                {
                                    link.HRef = "javascript:ShowFuture('" + (int)drv["TrailerResourceId"] + "', '" + (int)drv["TrailerResourceTypeId"] + "', '" + startOfRefusalDate.ToString("ddMMyyyyHHmm") + "');";
                                    link.Title = "Click to view this trailer's schedule.";
                                }
                                else
                                {
                                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                                    link.Title = "This is a third party trailer belonging to " + facOrganisation.GetNameForIdentityId(trailer.ThirdPartyOrganisationIdentityID).Replace("'", "''") + ".";
                                    link.HRef = "javascript:alert('" + link.Title + "');";
                                }
                                link.InnerText = "(" + ((string)drv["TrailerRef"]).Trim() + ")";
                            }
                            catch { }
                        }
                        break;
                }
            }
        }

        void grdRefusals_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();

            int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);
            int storePointId = cboStorePoint.SelectedValue == "" ? 0 : Convert.ToInt32(cboStorePoint.SelectedValue);

            DateTime startDate = rdiStartDate.SelectedDate.HasValue ? rdiStartDate.SelectedDate.Value : DateTime.MinValue;
            DateTime endDate = rdiEndDate.SelectedDate.HasValue ? rdiEndDate.SelectedDate.Value : DateTime.MinValue;
            //Set the date range to be the entire day
            endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));
            // Make sure the "At Store" option is selected if a store point has been specified.
            if (storePointId > 0)
                chkLocation.Items.FindByText(Utilities.UnCamelCase(eGoodsRefusedLocation.AtStore.ToString())).Selected = true;

            string goodStatus = GetStatusIdCSV();
            string location = GetLocationIdCSV();

            // if either date box is blank then ignore the dates.
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
                m_ds = facGoods.GetGoodsRefusedwithDates(clientId, startDate, endDate, goodStatus, location, storePointId);
            else
            {
                if (clientId == 0)
                {
                    if (goodStatus != "0" || location != "0" || storePointId > 0)
                        m_ds = facGoods.GetGoodsRefused(goodStatus, location, storePointId);
                    else
                    {
                        m_ds = new DataSet();
                        m_ds.Tables.Add(new DataTable());
                    }
                }
                else
                    m_ds = facGoods.GetGoodsRefusedwithParams(clientId, goodStatus, location, storePointId);
            }

            if (m_ds.Tables[0].Rows.Count != 0)
            {
                grdRefusals.DataSource = m_ds;

                btnReportBottom.Visible = btnReport.Visible = true;

                // We can only create a return job if only outstanding and at store are selected.
                int selectedStatesCount = 0;
                int selectedLocationsCount = 0;
                m_createJob = false;

                foreach (ListItem item in chkGoodsRefusedStatus.Items)
                    if (item.Selected)
                        selectedStatesCount++;

                foreach (ListItem item in chkLocation.Items)
                    if (item.Selected)
                        selectedLocationsCount++;

                if (selectedStatesCount == 1 && selectedLocationsCount == 1)
                {
                    if (chkGoodsRefusedStatus.SelectedValue == Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedStatus), eGoodsRefusedStatus.Outstanding)) &&
                        chkLocation.SelectedValue == Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedLocation), eGoodsRefusedLocation.AtStore)))
                        m_createJob = true;
                }

                // If all criteria matches show Include CheckBox column
                if (m_createJob && cboClient.SelectedValue != String.Empty)
                {
                    grdRefusals.Columns.FindByUniqueName("SelectColumn").Visible = true;
                }
                else
                {
                    grdRefusals.Columns.FindByUniqueName("SelectColumn").Visible = false;
                    
                }
            }
            else
            {
                grdRefusals.DataSource = m_ds;

                btnReportBottom.Visible = btnReport.Visible = false;
                reportViewer.Visible = false;
            }

            GoodsFilterOptions goodsFilterOptions = new GoodsFilterOptions();
            goodsFilterOptions.ClientId = clientId;
            goodsFilterOptions.LocationIdCSV = location;
            goodsFilterOptions.StatusIdCSV = goodStatus;
            goodsFilterOptions.DateFrom = startDate;
            goodsFilterOptions.DateTo = endDate;
            Session["GoodsFilterOptions"] = goodsFilterOptions;
        }

        void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        {
            GetData();
        }

        #endregion
	
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new System.EventHandler(this.ListInvoice_Init);
		}
		#endregion

		#region Populate Static Controls 
		///	<summary> 
		///	Populate Static Controls
		///	</summary>
        private void PopulateStaticControls()
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();

            // Filter Options
            rdoGoodsRefusedFilterType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedFilterType)));
            rdoGoodsRefusedFilterType.DataBind();
            rdoGoodsRefusedFilterType.Items[0].Selected = true;

            // Goods Refused Status
            chkGoodsRefusedStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedStatus)));
            chkGoodsRefusedStatus.DataBind();

            foreach (ListItem item in chkGoodsRefusedStatus.Items)
            {
                if (m_createJob)
                {
                    if (item.Text == "Outstanding")
                        item.Selected = true;
                }
                else
                    item.Selected = false;
            }

            Facade.IBusinessType facBT = new Facade.BusinessType();
            rcbBusinessType.DataSource = facBT.GetAll();
            rcbBusinessType.DataBind();

            // Current Location
            chkLocation.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedLocation)));
            chkLocation.DataBind();

            foreach (ListItem item in chkLocation.Items)
            {
                item.Selected = false;

                if (m_createJob)
                {
                    if (item.Text == "At Store")
                        item.Selected = true;
                }
                else
                    item.Selected = false;
            }
        }

        #endregion

        #region Combo's Server Methods

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

        }

        void cboStorePoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboStorePoint.Items.Clear();
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString()  != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                try { identityId = int.Parse(values[0]); }
                catch { }
                if (values.Length > 1 && values[1] != "false" && !string.IsNullOrEmpty(values[1]))
                {
                    searchText = values[1];
                }
                else if (!string.IsNullOrEmpty(e.Text))
                    searchText = e.Text;
            }
            else
                searchText = e.Context["FilterString"].ToString();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, 0, searchText);
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboStorePoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboStoreOrganisation_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboStoreOrganisation.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboStoreOrganisation.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

        }		

		#endregion 
		
		#region Events

		///	<summary> 
		///	Button Search Click
		///	</summary>
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
            reportViewer.Visible = false;

			if (Page.IsValid)
				GetData();
		}

        void btnFind_Click(object sender, EventArgs e)
        {
            reportViewer.Visible = false;

            if (Page.IsValid)
                GetData();
        }

        private void btnReport_Click(object sender, System.EventArgs e)
        {
            LoadGoodsRefusedReport();
        }
		
		private void btnCreateCollection_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
                string refusalIdCSV = GetRefusalsForReturnJob();
                
				Response.Redirect("../Job/addupdategoodsreturnjob.aspx?RefusalIdCSV=" + refusalIdCSV);
			}
		}

		private void ClearFields()
		{
			btnReportBottom.Visible = btnReport.Visible = false;
			reportViewer.Visible = false;

			for(int i = 0; i < chkGoodsRefusedStatus.Items.Count; i++)
			{
				chkGoodsRefusedStatus.Items[i].Selected = false;
			}

			for(int i = 0; i < chkLocation.Items.Count; i++)
			{
				chkLocation.Items[i].Selected = false;
			}
		}

		private void cboGoodsRefusedState_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ClearFields(); 
		}

		private void cboGoodsRefusedStatus_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ClearFields();
		}
		
        #endregion

		#region Methods

        private void GetData()
        {
            pnlGoodsRefusedListBoxes1.Visible = pnlGoodsRefusedListBoxes2.Visible = true;

            grdRefusals.Rebind();
        }

		private string GetStatusIdCSV()
		{
			string statusIdCSV = string.Empty;

			// If none return 0 else return what has been selected (i.e. 2,3,4)
			for ( int i = 0; i < chkGoodsRefusedStatus.Items.Count; i++ )
			{
				if (chkGoodsRefusedStatus.Items[i].Selected)
				{
					if (statusIdCSV != string.Empty)
						statusIdCSV = statusIdCSV + ", " + ((int) Enum.Parse(typeof(eGoodsRefusedStatus), chkGoodsRefusedStatus.Items[i].Value.ToString().Replace(" ", ""))).ToString(); 
					else
						statusIdCSV = ((int) Enum.Parse(typeof(eGoodsRefusedStatus), chkGoodsRefusedStatus.Items[i].Value.ToString().Replace(" ", ""))).ToString(); 
				}
			}
			if  (statusIdCSV == string.Empty)
				statusIdCSV = "0";
			
			return statusIdCSV;
		}

		private string GetLocationIdCSV()
		{
			string locationIdCSV = string.Empty;

			// If none return 0 else return what has been selected (i.e. 2,3,4)
			for ( int i = 0; i < chkLocation.Items.Count; i++ )
			{
				if (chkLocation.Items[i].Selected)
				{
					if (locationIdCSV != string.Empty)
						locationIdCSV = locationIdCSV + ", " + ((int) Enum.Parse(typeof(eGoodsRefusedLocation), chkLocation.Items[i].Value.ToString().Replace(" ", ""))).ToString(); 
					else
						locationIdCSV = ((int) Enum.Parse(typeof(eGoodsRefusedLocation), chkLocation.Items[i].Value.ToString().Replace(" ", ""))).ToString(); 
				}
			}
			if  (locationIdCSV == string.Empty)
				locationIdCSV = "0";
			
			return locationIdCSV;
		}

		private void LoadGoodsRefusedReport()
		{
			// Configure the Session variables used to pass data to the report
			NameValueCollection reportParams = new NameValueCollection();
			
			// Client Name & Id			
			if (cboClient.SelectedValue != "")
			{
				reportParams.Add("Client", Convert.ToString (cboClient.Text));
				reportParams.Add("ClientId", Convert.ToString(cboClient.SelectedValue));
			}
			
			// Date Range
			if (Convert.ToDateTime(Session["StartDate"]).Date != DateTime.MinValue)
				reportParams.Add("startDate", Convert.ToDateTime(Session["StartDate"]).ToString("dd/MM/yy"));
			
			if (Convert.ToDateTime(Session["EndDate"]).Date != DateTime.MinValue)
				reportParams.Add("endDate", Convert.ToDateTime(Session["EndDate"]).ToString("dd/MM/yy"));

			reportParams.Add("RefusalIds", GetRefusalsForReturnJob());
            GetData();

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.GoodsRefusal;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = m_ds;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
							
			// Show the user control

			if (cboClient.SelectedValue != "")
				reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);

			reportViewer.Visible = true;
		}

        private string ConvertApostrophesToASCII(string originalString)
        {
            return originalString.Replace("'", "&#39"); // This is the ASCII Numerical Code for the ' symbol.
        }

        #endregion

		#region Refused Goods Grid
	
        protected string GetRefusalsForReturnJob()
        {
            StringBuilder working = new StringBuilder();

            foreach (GridItem gi in grdRefusals.Items)
            {
                if (gi is GridDataItem)
                {
                    CheckBox chkInclude = (CheckBox)gi.FindControl("chkInclude");
                    if (chkInclude.Checked)
                    {
                        if (working.Length > 0)
                            working.Append(",");

                        working.Append(gi.OwnerTableView.DataKeyValues[gi.ItemIndex]["RefusalId"].ToString());
                    }
                }
            }

            return working.ToString();
        }
		
        #endregion

        #region WebMethods

        [System.Web.Services.WebMethod]
        public static string CreateReturn(List<int> refusalIDs, int businessTypeID, bool changeDeliveryDetails, int changeDeliveryPointID, DateTime? changeDeliveryFromDate, DateTime? changeDeliveryFromTime, DateTime? changeDeliveryByDate, DateTime? changeDeliveryByTime, bool deliveryIsAnytime, int driverResourceID, int VehicleResourceID, int trailerResourceID, int planningCatgeoryID, string userName, int userIdentityID, bool createManifest, string resourceName, int? subcontractorID, int? subcontractType, decimal? subcontractRate, bool showAsCommunicated, string manifestTitle, DateTime? manifestDate, bool createRun, DateTime? collectionFromDate, DateTime? collectionFromTime, DateTime? collectionByDate, DateTime? collectionByTime, bool collectionIsAnytime, bool leaveGoods, int? leavePointId, DateTime? leaveFromDate, DateTime? leaveFromTime, DateTime? leaveByDate, DateTime? leaveByTime, bool leaveIsAnytime)
        {
            if (subcontractorID.HasValue && subcontractorID == Globals.Configuration.PalletNetworkID && subcontractType.HasValue && ((Orchestrator.Entities.eSubContractorDataItem)subcontractType.Value) == Orchestrator.Entities.eSubContractorDataItem.Job)
                subcontractType = (int)Orchestrator.Entities.eSubContractorDataItem.Order;

            string retVal = "";

            #region Prepare Orders

            // If we are planning the deliveries before the collection and are planning to pick these up from a cross dock location set the relevant information
            DateTime? deliveryFromDateTime = null, deliveryByDateTime = null;
            if (changeDeliveryDetails)
            {
                deliveryFromDateTime = changeDeliveryFromDate.Value;

                if (changeDeliveryFromTime.HasValue)
                    deliveryFromDateTime = deliveryFromDateTime.Value.Add(changeDeliveryFromTime.Value.TimeOfDay);

                deliveryByDateTime = changeDeliveryByDate.Value;

                if (changeDeliveryByTime.HasValue)
                    deliveryByDateTime = deliveryByDateTime.Value.Add(changeDeliveryByTime.Value.TimeOfDay);
            }

            DateTime collectionFromDateTime = new DateTime(collectionFromDate.Value.Year, collectionFromDate.Value.Month, collectionFromDate.Value.Day, collectionFromTime.Value.Hour, collectionFromTime.Value.Minute, 0);
            DateTime collectionByDateTime = new DateTime(collectionByDate.Value.Year, collectionByDate.Value.Month, collectionByDate.Value.Day, collectionByTime.Value.Hour, collectionByTime.Value.Minute, 0);

            DateTime? leaveFromDateTime = null;
            DateTime? leaveByDateTime = null;
            if (leaveGoods)
            {
                leaveFromDateTime = new DateTime(leaveFromDate.Value.Year, leaveFromDate.Value.Month, leaveFromDate.Value.Day, leaveFromTime.Value.Hour, leaveFromTime.Value.Minute, 0);
                leaveByDateTime = new DateTime(leaveByDate.Value.Year, leaveByDate.Value.Month, leaveByDate.Value.Day, leaveByTime.Value.Hour, leaveByTime.Value.Minute, 0);
            }            

            #endregion
            
            try
            {
                string orderIDs = string.Empty;

                // Create Orders, if flagged, create Run as well.
                Facade.IGoodsRefusal facGoodsRes = new Facade.GoodsRefusal();
                Entities.FacadeResult res = facGoodsRes.CreateReturn(refusalIDs, createRun, businessTypeID, changeDeliveryDetails, changeDeliveryPointID, deliveryFromDateTime, deliveryByDateTime, deliveryIsAnytime, collectionFromDateTime, collectionByDateTime, collectionIsAnytime, leaveGoods, leavePointId, leaveFromDateTime, leaveByDateTime, leaveIsAnytime, userName, out orderIDs);

                if (!res.Success)
                {
                    string msg = "Could not create return due to the following infringements:\n" + string.Join("\n", res.Infringements.Select(i => i.Description));
                    throw new ApplicationException(msg);
                }

                if (createRun)
                {
                    int jobID = res.ObjectId;
                    List<int> localOrderIDs = new List<int>();

                    retVal = "j:" + res.ObjectId.ToString();

                    string[] oIDs = orderIDs.Split(',');
                    foreach (string s in oIDs)
                        localOrderIDs.Add(int.Parse(s));

                    #region Run Options

                    // Reload to prevent failing br test
                    Entities.Job job = null;
                    List<int> instructionIDs = new List<int>();

                    Facade.IJob facJob = new Facade.Job();
                    job = facJob.GetJob(jobID, true);

                    foreach (Entities.Instruction instruction in job.Instructions)
                        instructionIDs.Add(instruction.InstructionID);

                    Facade.IInstruction facInstruction = new Facade.Instruction();

                    if (subcontractorID.HasValue && subcontractorID.Value > 0)
                    {
                        DataAccess.IBusinessType dacBusinessType = new DataAccess.BusinessType();
                        DataSet _dsBusinessType = dacBusinessType.GetAll();
                        _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
                        DataRow drBusinessType = _dsBusinessType.Tables[0].Rows.Find(businessTypeID);

                        #region Sub contract the job

                        Entities.eSubContractorDataItem subOutChoice = Entities.eSubContractorDataItem.Job;
                        if (subcontractType.HasValue)
                            subOutChoice = (Entities.eSubContractorDataItem)subcontractType.Value;

                        Entities.JobSubContractor jobSubContractor = new Entities.JobSubContractor();
                        Facade.IOrganisation facOrg = new Facade.Organisation();
                        Entities.Organisation org = facOrg.GetForIdentityId(subcontractorID.Value);
                        Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

                        jobSubContractor.JobID = jobID;
                        jobSubContractor.SubContractWholeJob = (subOutChoice == Entities.eSubContractorDataItem.Job);
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
                        if (subOutChoice == Entities.eSubContractorDataItem.Order)
                            res = facJobSubContractor.Create(jobID, new List<int>(), localOrderIDs, jobSubContractor, DateTime.Now, userName, true);
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

                            rm.ResourceManifestJobs = new List<Entities.ResourceManifestJob>();
                            int jobOrder = 0;
                            foreach (Entities.Instruction insruction in job.Instructions)
                            {
                                rmj = new Entities.ResourceManifestJob();
                                rmj.InstructionId = insruction.InstructionID;
                                rmj.JobId = jobID;
                                rmj.JobOrder = jobOrder;
                                rm.ResourceManifestJobs.Add(rmj);
                                jobOrder++;
                            }
                            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
                            rm.ResourceManifestId = facResourceManifest.CreateResourceManifest(rm, userName);

                            retVal += "|" + rm.ResourceManifestId.ToString() + "|1";
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

                        if (planningCatgeoryID > 0 || driverResourceID > 0 || VehicleResourceID > 0 || trailerResourceID > 0)
                        {
                            if (planningCatgeoryID > 0)
                                res = facInstruction.AssignPlanningCategory(instructionIDs, jobID, planningCatgeoryID, DateTime.Now, userName);
                            else
                                res = facInstruction.PlanInstruction(instructionIDs, jobID, driverResourceID, VehicleResourceID, trailerResourceID, DateTime.Now, userName);

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
                                    rm.ResourceManifestJobs = new List<Entities.ResourceManifestJob>();

                                    int jobOrder = 0;
                                    foreach (Entities.Instruction insruction in job.Instructions)
                                    {
                                        rmj = new Entities.ResourceManifestJob();
                                        rmj.InstructionId = insruction.InstructionID;
                                        rmj.JobId = jobID;
                                        rmj.JobOrder = jobOrder;
                                        rm.ResourceManifestJobs.Add(rmj);
                                        jobOrder++;
                                    }

                                    Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
                                    rm.ResourceManifestId = facResourceManifest.CreateResourceManifest(rm, userName);

                                    retVal += "|" + rm.ResourceManifestId.ToString();
                                }
                                // Show be able to show as communicated event if we are not creating a manifest
                                if (res.Success && showAsCommunicated)
                                {
                                    // mark the instructions as communicated so that the instruction shows as in progress
                                    CommunicateInstructions(jobID, driverResourceID, VehicleResourceID, userName);
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    retVal = "o:" + orderIDs;
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

        #region Private Functions

        private static bool CommunicateInstructions(int jobID, int driverID, int vehicleID, string userId)
        {
            Entities.DriverCommunication communication = new Entities.DriverCommunication();
            communication.Comments = "Communicated via Goods Refused List";
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
    }
}

