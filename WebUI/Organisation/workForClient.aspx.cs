using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Telerik.Web.UI;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Organisation
{
    public partial class Organisation_workForClient : Orchestrator.Base.BasePage
    {
        private const string c_AllWorkForOrganisation_VS = "vs_AllWorkForOrganisation";
        private const string c_IsClient_VS = "vs_IsClient";
        private const string _extraChargesTemplate = "{0} extra{1} worth {2}";
        private const string _extraBreakdownTemplate = @"showExtrasBreakdown(this, '  <table cellpadding=""0"" cellspacing=""1"" class=""Grid"" width=""260""><thead><tr><th width=""120"">State</th><th align=""right"" width=""60"">Count</th><th align=""right"" width=""80"">Amount</th></tr></thead><tbody><tr><td>Awaiting Acceptance</td><td align=""right"">{0}</td><td align=""right"">{1}</td></tr><tr><td>Accepted</td><td align=""right"">{2}</td><td align=""right"">{3}</td></tr><tr><td>Refused</td><td align=""right"">{4}</td><td align=""right"">{5}</td></tr><tr><td>Invoiced</td><td align=""right"">{6}</td><td align=""right"">{7}</td></tr></tbody><tfoot><tr><td>&nbsp;</td><td align=""right""><b>{8}</b></td><td align=""right""><b>{9}</b></td></tr></tfoot></table>')";
        private const string c_Mode_QS = "mode";
        private const string c_customerID_QS = "customerID";
        private const string c_startDate_QS = "startDate";
        private const string c_endDate_QS = "endDate";
        private Entities.Organisation organisation = null;

        #region Properties
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
        public DataSet AllWorkForOrganisation
        {
            get
            {
                if (ViewState[c_AllWorkForOrganisation_VS] == null)
                {
                    int collectionPointID, deliveryPointID;
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    int.TryParse(ucCollectionPoint.PointID.ToString(), out collectionPointID);
                    int.TryParse(ucDeliveryPoint.PointID.ToString(), out deliveryPointID);

                    var selectedClients = hidSelectedClientsValues.Value;

                    if (selectedClients.Length > 0)
                        ViewState[c_AllWorkForOrganisation_VS] = facOrg.GetAllWorkForClients(selectedClients, this.rdiStartDate.SelectedDate.Value, this.rdiEndDate.SelectedDate.Value, collectionPointID, deliveryPointID, cboSearchAgainstWorker.Items[0].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[1].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[2].Selected || cboSearchAgainstWorker.Items[3].Selected);
                    else
                        ViewState[c_AllWorkForOrganisation_VS] = null;
                }

                return (DataSet)ViewState[c_AllWorkForOrganisation_VS];
            }
            set { ViewState[c_AllWorkForOrganisation_VS] = value; }
        }

        public bool IsClient
        {
            get
            {
                if (ViewState[c_IsClient_VS] == null)
                    return true;
                else
                    return (bool)ViewState[c_IsClient_VS];
            }
            set { ViewState[c_IsClient_VS] = value; }
        }

        #endregion

        #region Page Setup

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString[c_Mode_QS]))
            {
                switch (Request.QueryString[c_Mode_QS].ToLower())
                {
                    case "client":
                        IsClient = true;
                        break;
                    case "subcontractor":
                        IsClient = false;
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

            this.repBusinessType.ItemDataBound += new RepeaterItemEventHandler(repBusinessType_ItemDataBound);

            this.grdNormal.NeedDataSource += new GridNeedDataSourceEventHandler(grdNormal_NeedDataSource);
            this.grdNormal.ItemDataBound += new GridItemEventHandler(grdNormal_ItemDataBound);
            this.grdSummary.NeedDataSource += new GridNeedDataSourceEventHandler(grdSummary_NeedDataSource);
            this.grdSummary.ItemDataBound += new GridItemEventHandler(grdSummary_ItemDataBound);
        }

        static void repBusinessType_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTitle = (Label)e.Item.FindControl("lblTitle");
                HiddenField hidBusinessTypeID = (HiddenField)e.Item.FindControl("hidBusinessTypeID");

                hidBusinessTypeID.Value = ((DataRowView)(e.Item.DataItem)).Row["BusinessTypeID"].ToString();
                lblTitle.Text = "Groupage " + ((DataRowView)(e.Item.DataItem)).Row["Description"];
            }
        }

        private void ConfigureDisplay()
        {

            if (!string.IsNullOrEmpty(Request.QueryString[c_customerID_QS]))
            {
                int id = int.Parse(Request.QueryString[c_customerID_QS]);
                Facade.IOrganisation facOrganisation = new Facade.Organisation();

                organisation = facOrganisation.GetForIdentityId(id);

                RadComboBoxItem listItem = new RadComboBoxItem(organisation.OrganisationDisplayName, organisation.IdentityId.ToString());
                cboClient.Items.Add(listItem);
                cboClient.SelectedValue = organisation.IdentityId.ToString();

                hidSelectedClientsValues.Value = organisation.IdentityId.ToString() + ",";
                hidSelectedClientsText.Value = organisation.OrganisationDisplayName  + ",";

            }

            DateTime startDate = DateTime.Today.AddMonths(-1);
            System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar();


            if (!string.IsNullOrEmpty(Request.QueryString[c_startDate_QS]))
            {
                this.rdiStartDate.SelectedDate = DateTime.Parse(Request.QueryString[c_startDate_QS],  null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            else
            {
                this.rdiStartDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, 01);
            }

            if (!string.IsNullOrEmpty(Request.QueryString[c_endDate_QS]))
            {
                this.rdiEndDate.SelectedDate = DateTime.Parse(Request.QueryString[c_endDate_QS], null, System.Globalization.DateTimeStyles.RoundtripKind);
            }
            else
            {
                this.rdiEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, cal.GetDaysInMonth(startDate.Year, startDate.Month));
            }

           
            lblMode.Text = IsClient ? "Client" : "Sub-Contractor";

            if (!string.IsNullOrEmpty(cboClient.SelectedValue) &&
                !string.IsNullOrEmpty(Request.QueryString[c_startDate_QS]) &&
                !string.IsNullOrEmpty(Request.QueryString[c_endDate_QS]))
            {
                Refresh();
            }

            this.rfvCboClient.Enabled = !IsClient;

            seSelectedClients.Visible = lblSelectedClients.Visible = btnRemoveSelectedClients.Visible = btnAddCboClient.Visible = IsClient;
        }

        #endregion

        #region Events

        #region ComboBox

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            LoadClients(e);
        }

        private void LoadClients(RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds;

            if (IsClient)
                ds = facReferenceData.GetAllClientsFiltered(e.Text);
            else
                ds = facReferenceData.GetAllSubContractorsFiltered(e.Text);

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

        #region Buttons

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion

        #region Grids

        private void BindRepeater()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            DataSet dsBusinessType;

            int identityID;
            int.TryParse(cboClient.SelectedValue, out identityID);

            organisation = facOrganisation.GetForIdentityId(identityID);

            dsBusinessType = facBusinessType.GetAll();

            repBusinessType.DataSource = dsBusinessType;
            repBusinessType.DataBind();
            grdNormal.Rebind();
            grdSummary.Rebind();
        }

        protected void grd_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RadGrid grdBusinessType = (RadGrid)source;
            HiddenField hidBusinessTypeID = (HiddenField)grdBusinessType.Parent.FindControl("hidBusinessTypeID");

            RadGrid grid = (RadGrid)source;

            if (organisation != null)
                if (organisation.OrganisationType == eOrganisationType.SubContractor)
                    grid.MasterTableView.Columns.FindByUniqueName("chkReference").Display = false;

            if (hidBusinessTypeID != null && grdBusinessType != null && AllWorkForOrganisation != null)
            {
                string businessTypeID = string.Empty;

                if (hidBusinessTypeID.Value == string.Empty)
                {
                    businessTypeID = ((System.Data.DataRowView)((RepeaterItem)grdBusinessType.Parent).DataItem).Row["BusinessTypeID"].ToString();
                    hidBusinessTypeID.Value = businessTypeID;
                }
                else
                    businessTypeID = hidBusinessTypeID.Value;

                AllWorkForOrganisation.Tables[0].DefaultView.RowFilter = "BusinessTypeID = " + businessTypeID;
                grdBusinessType.DataSource = AllWorkForOrganisation.Tables[0].DefaultView;
            }
        }

        protected void grd_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                CheckBox chk = e.Item.FindControl("chkHeader") as CheckBox;
                if (chk != null)
                    chk.Attributes.Add("onclick",
                                           string.Format("javascript:HandleGridSelection({0});", ((RadGrid)sender).ClientID));
            }

            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)(e.Item.DataItem);
                e.Item.Style["background-color"] = Orchestrator.WebUI.Utilities.GetJobStateColourForHTML((eJobState)drv.Row["JobStateID"]);

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

                HtmlGenericControl spnExtraCharges = (HtmlGenericControl)e.Item.FindControl("spnExtraCharges");
                DisplayExtrasInformation(drv.Row, spnExtraCharges);

                HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces");
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

                CheckBox chk = e.Item.FindControl("chkRow") as CheckBox;
                if (chk != null)
                {
                    if ((int)drv.Row["JobStateID"] == (int)eJobState.Invoiced || (int)drv.Row["JobStateID"] == (int)eJobState.Cancelled || (bool)drv.Row["IsBeingInvoiced"])
                        chk.Enabled = false;
                    else
                        chk.Attributes.Add("onclick",
                                           string.Format("javascript:ChangeOrderList(event, this, {0});", ((DataRowView)e.Item.DataItem)["OrderID"].ToString()));
                }
            }

            if (e.Item is GridFooterItem)
            {
                Telerik.Web.UI.RadGrid grdBusinessType = (Telerik.Web.UI.RadGrid)sender;
                HiddenField hidBusinessTypeID = (HiddenField)grdBusinessType.Parent.FindControl("hidBusinessTypeID");

                AllWorkForOrganisation.Tables[2].DefaultView.RowFilter = "BusinessTypeID = " + hidBusinessTypeID.Value;
                Label lblTotalOrderCharge = (Label)e.Item.FindControl("lblTotalOrderCharge");

                if (AllWorkForOrganisation.Tables[2].DefaultView.Count == 0)
                    lblTotalOrderCharge.Text = 0d.ToString("C");
                else
                {
                    DataRow summaryRow = AllWorkForOrganisation.Tables[2].DefaultView[0].Row;
                    Decimal totalValue = Convert.ToDecimal(summaryRow["Total Rate"]);
                    lblTotalOrderCharge.Text = totalValue.ToString("C");

                    HtmlGenericControl spnExtrasBreakdown = (HtmlGenericControl)e.Item.FindControl("spnExtrasBreakdown");
                    DisplayExtrasInformation(summaryRow, spnExtrasBreakdown);
                }
            }
        }

        void grdNormal_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (organisation != null)
                if (organisation.OrganisationType == eOrganisationType.SubContractor)
                    grdNormal.MasterTableView.Columns.FindByUniqueName("chkReference").Display = false;
                else
                    grdNormal.MasterTableView.Columns.FindByUniqueName("chkReference").Display = true;

            if (string.IsNullOrEmpty(Request.QueryString["rcbID"]) && AllWorkForOrganisation != null)
            {
                AllWorkForOrganisation.Tables[0].DefaultView.RowFilter = "BusinessTypeID = 0";

                if (AllWorkForOrganisation.Tables[0].DefaultView.Count > 0)
                {
                    grdNormal.DataSource = AllWorkForOrganisation.Tables[0].DefaultView;
                    tblNormal.Style["display"] = "";
                }
                else
                    tblNormal.Style["display"] = "none";
            }
        }

        void grdNormal_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                CheckBox chk = e.Item.FindControl("chkHeader") as CheckBox;
                if (chk != null)
                    chk.Attributes.Add("onclick",
                                           string.Format("javascript:HandleGridSelection({0});", ((RadGrid)sender).ClientID));
            }

            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)(e.Item.DataItem);
                e.Item.Style["background-color"] = Orchestrator.WebUI.Utilities.GetJobStateColourForHTML((eJobState)drv["JobStateID"]);

                if (((bool)drv["HasPOD"]))
                    try
                    {
                        HyperLink lnkPOD = (HyperLink)e.Item.FindControl("lnkPOD");
                        lnkPOD.ForeColor = System.Drawing.Color.Blue;
                        lnkPOD.NavigateUrl = drv.Row["ScannedFormPDF"].ToString().Trim();
                    }
                    catch (Exception ex)
                    { }

                HtmlGenericControl spnExtraCharges = (HtmlGenericControl)e.Item.FindControl("spnExtraCharges");
                DisplayExtrasInformation(drv.Row, spnExtraCharges);

                CheckBox chk = e.Item.FindControl("chkRow") as CheckBox;
                if (chk != null)
                {
                    if ((int)drv.Row["JobStateID"] == (int)eJobState.Invoiced || (int)drv.Row["JobStateID"] == (int)eJobState.Cancelled || (bool)drv.Row["IsBeingInvoiced"])
                        chk.Enabled = false;
                    else
                        chk.Attributes.Add("onclick",
                                           string.Format("javascript:ChangeJobList(event, this, {0});", ((DataRowView)e.Item.DataItem)["JobID"].ToString()));
                }

                int jobID = (int)drv["JobID"];
                string deliveryOrderNumber = (string)drv["DeliveryOrderNumber"];

                DataRow[] otherRowsForThisJob =
                    AllWorkForOrganisation.Tables[0].Select("BusinessTypeID = 0 AND JobID = " +
                                                                      jobID.ToString() + " AND DeliveryOrderNumber <> '" +
                                                                      deliveryOrderNumber + "'");

                HtmlGenericControl spnPalletSpaces = (HtmlGenericControl)e.Item.FindControl("spnPalletSpaces");
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

                if (otherRowsForThisJob.Length > 0)
                {
                    // This job's rate information may already have been displayed in the grid.
                    GridDataItem[] gdi = new GridDataItem[grdNormal.Items.Count];
                    grdNormal.Items.CopyTo(gdi, 0);
                    List<GridDataItem> normalItems = new List<GridDataItem>(gdi);

                    if (normalItems.Find(delegate(GridDataItem testItem)
                            {
                                bool match = (int)((DataRowView)testItem.DataItem)["JobID"] == jobID;
                                return match;
                            }) != null
                    )
                    {
                        HtmlGenericControl spnCharge = (HtmlGenericControl)e.Item.FindControl("spnCharge");
                        HtmlGenericControl spnNoPallets = (HtmlGenericControl)e.Item.FindControl("spnNoPallets");
                        spnCharge.Visible = false;
                        spnExtraCharges.Visible = false;
                        spnNoPallets.Visible = false;
                    }
                }
            }

            if (e.Item is GridFooterItem)
            {
                AllWorkForOrganisation.Tables[2].DefaultView.RowFilter = "BusinessTypeID = 0";
                Label lblTotalJobCharge = (Label)e.Item.FindControl("lblTotalJobCharge");

                if (AllWorkForOrganisation.Tables[2].DefaultView.Count == 0)
                {
                    lblTotalJobCharge.Text = 0d.ToString("C");
                }
                else
                {
                    DataRow summaryRow = AllWorkForOrganisation.Tables[2].DefaultView[0].Row;
                    Decimal totalValue = Convert.ToDecimal(summaryRow["Total Rate"]);
                    lblTotalJobCharge.Text = totalValue.ToString("C");

                    HtmlGenericControl spnExtrasBreakdown = (HtmlGenericControl)e.Item.FindControl("spnExtrasBreakdown");
                    DisplayExtrasInformation(summaryRow, spnExtrasBreakdown);
                }
            }
        }

        #region Methods that help display extras information

        private static void DisplayExtrasInformation(DataRow row, HtmlContainerControl container)
        {
            int countExtrasAwaitingResponse = (int)row["CountExtrasAwaitingResponse"];
            int countExtrasAccepted = (int)row["CountExtrasAccepted"];
            int countExtrasRefused = (int)row["CountExtrasRefused"];
            int countExtrasInvoiced = (int)row["CountExtrasInvoiced"];
            decimal totalExtrasAwaitingResponse = (decimal)row["TotalExtrasAwaitingResponse"];
            decimal totalExtrasAccepted = (decimal)row["TotalExtrasAccepted"];
            decimal totalExtrasRefused = (decimal)row["TotalExtrasRefused"];
            decimal totalExtrasInvoiced = (decimal)row["TotalExtrasInvoiced"];

            DisplayExtrasInformation(countExtrasAwaitingResponse, countExtrasAccepted, countExtrasRefused, countExtrasInvoiced, totalExtrasAwaitingResponse, totalExtrasAccepted, totalExtrasRefused, totalExtrasInvoiced, container);
        }

        private static void DisplayExtrasInformation(int countExtrasAwaitingResponse, int countExtrasAccepted, int countExtrasRefused, int countExtrasInvoiced, decimal totalExtrasAwaitingResponse, decimal totalExtrasAccepted, decimal totalExtrasRefused, decimal totalExtrasInvoiced, HtmlContainerControl container)
        {
            int totalNumberOfExtras = countExtrasAwaitingResponse +
                                      countExtrasAccepted + countExtrasRefused +
                                      countExtrasInvoiced;
            decimal totalAmountOnExtras = totalExtrasAwaitingResponse +
                                          totalExtrasAccepted +
                                          totalExtrasRefused +
                                          totalExtrasInvoiced;

            if (totalNumberOfExtras == 0)
            {
                container.InnerHtml = string.Empty;
                container.Attributes.Remove("onMouseOver");
            }
            else
            {
                container.InnerHtml =
                    string.Format(_extraChargesTemplate, totalNumberOfExtras,
                                  totalNumberOfExtras == 1 ? "" : "s", totalAmountOnExtras.ToString("C"));
                container.Attributes.Add("onMouseOver",
                                         string.Format(_extraBreakdownTemplate,
                                                       countExtrasAwaitingResponse,
                                                       totalExtrasAwaitingResponse.ToString("C"),
                                                       countExtrasAccepted,
                                                       totalExtrasAccepted.ToString("C"),
                                                       countExtrasRefused,
                                                       totalExtrasRefused.ToString("C"),
                                                       countExtrasInvoiced,
                                                       totalExtrasInvoiced.ToString("C"),
                                                       totalNumberOfExtras,
                                                       totalAmountOnExtras.ToString("C")));
            }
        }

        #endregion

        void grdSummary_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["rcbID"]) && AllWorkForOrganisation != null)
            {
                if (AllWorkForOrganisation.Tables[1].DefaultView.Count > 0)
                {
                    grdSummary.DataSource = AllWorkForOrganisation.Tables[1].DefaultView;
                    tblSummary.Style["display"] = "";
                }
                else
                    tblSummary.Style["display"] = "none";
            }
        }

        void grdSummary_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                HtmlGenericControl spnTotalExtraCharges =
                    (HtmlGenericControl)e.Item.FindControl("spnTotalExtraCharges");
                DisplayExtrasInformation(((DataRowView)e.Item.DataItem).Row, spnTotalExtraCharges);
            }
            else if (e.Item is GridFooterItem)
            {
                Label lblTotalOfFuelSurchargeAmount = (Label)e.Item.FindControl("lblTotalOfFuelSurchargeAmount");
                Label lblTotalCountOfDeliveryRuns = (Label)e.Item.FindControl("lblTotalCountOfDeliveryRuns");
                Label lblTotalCountOfJobs = (Label)e.Item.FindControl("lblTotalCountOfJobs");
                Label lblTotalRate = (Label)e.Item.FindControl("lblTotalRate");
                Label lblTotalWeight = (Label)e.Item.FindControl("lblTotalWeight");
                HtmlGenericControl spnTotalExtrasBreakdown =
                    (HtmlGenericControl)e.Item.FindControl("spnTotalExtrasBreakdown");

                int totalCountOfJobs = 0;
                decimal totalRate = 0;
                int totalWeight = 0;
                int countExtrasAwaitingResponse = 0;
                int countExtrasAccepted = 0;
                int countExtrasRefused = 0;
                int countExtrasInvoiced = 0;
                decimal totalExtrasAwaitingResponse = 0;
                decimal totalExtrasAccepted = 0;
                decimal totalExtrasRefused = 0;
                decimal totalExtrasInvoiced = 0;
                decimal totalFuelSurcharge = 0;
                int countOfDeliveryRuns = 0;

                foreach (DataRow row in AllWorkForOrganisation.Tables[2].Rows)
                {
                    totalCountOfJobs += (int)row["CountOfJobs"];
                    totalRate += (decimal)row["Total Rate"];
                    totalWeight += (int)(decimal)row["TotalWeight"];
                    totalFuelSurcharge += (decimal)row["TotalFuelSurcharge"];
                    countOfDeliveryRuns += (int)row["RunCount"];
                    countExtrasAwaitingResponse += (int)row["CountExtrasAwaitingResponse"];
                    countExtrasAccepted += (int)row["CountExtrasAccepted"];
                    countExtrasRefused += (int)row["CountExtrasRefused"];
                    countExtrasInvoiced += (int)row["CountExtrasInvoiced"];
                    totalExtrasAwaitingResponse += (decimal)row["TotalExtrasAwaitingResponse"];
                    totalExtrasAccepted += (decimal)row["TotalExtrasAccepted"];
                    totalExtrasRefused += (decimal)row["TotalExtrasRefused"];
                    totalExtrasInvoiced += (decimal)row["TotalExtrasInvoiced"];
                }

                lblTotalCountOfJobs.Text = totalCountOfJobs.ToString();
                lblTotalRate.Text = totalRate.ToString("C");
                lblTotalWeight.Text = totalWeight.ToString();
                lblTotalCountOfDeliveryRuns.Text = countOfDeliveryRuns.ToString();
                lblTotalOfFuelSurchargeAmount.Text = totalFuelSurcharge.ToString("C");
                DisplayExtrasInformation(countExtrasAwaitingResponse, countExtrasAccepted, countExtrasRefused,
                                         countExtrasInvoiced, totalExtrasAwaitingResponse, totalExtrasAccepted,
                                         totalExtrasRefused, totalExtrasInvoiced, spnTotalExtrasBreakdown);
            }
        }

        private void Refresh()
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();
            int identityID, collectionPointID, deliveryPointID;
            int.TryParse(cboClient.SelectedValue, out identityID);
            int.TryParse(ucCollectionPoint.PointID.ToString(), out collectionPointID);
            int.TryParse(ucDeliveryPoint.PointID.ToString(), out deliveryPointID);

            var selectedClients = hidSelectedClientsValues.Value;

            if (IsClient)
                AllWorkForOrganisation = facOrg.GetAllWorkForClients(selectedClients, rdiStartDate.SelectedDate.Value, rdiEndDate.SelectedDate.Value, collectionPointID, deliveryPointID, cboSearchAgainstWorker.Items[0].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[1].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[2].Selected || cboSearchAgainstWorker.Items[3].Selected);
            else
                AllWorkForOrganisation = facOrg.GetAllWorkForSubContractor(identityID, rdiStartDate.SelectedDate.Value, rdiEndDate.SelectedDate.Value, collectionPointID, deliveryPointID, cboSearchAgainstWorker.Items[0].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[1].Selected || cboSearchAgainstWorker.Items[3].Selected, cboSearchAgainstWorker.Items[2].Selected || cboSearchAgainstWorker.Items[3].Selected);

            BindRepeater();
        }

        #endregion



        #endregion


    }
}
