using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using P1TP.Components.Web.Validation;

using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using System.Text;

namespace Orchestrator.WebUI.Resource.PCV
{
    /// <summary>
    /// Summary description for List PCV.
    /// </summary>
    public partial class ListPCV : Orchestrator.Base.BasePage
    {
        #region Constants and Enums

        public enum eDataGridColumns { PCVId, PCVNo, ViewPCV, PointName, DateOfIssue, NoOfPallets, NoOfSignings, PCVRedemptionStatus, PCVStatus, RequiresScan, JobId, LoadNo, OrganisationName, ClientsCustomer, ActualDeliveryDate, FullName, EmailFax };

        #endregion

        #region Page Variables
        private const string C_PCVDATA_VS = "PCVData";
        private DataSet dsPCV;
        private int m_jobId;
        private int m_pcvId;
        //private int runningTotal = 0;
        private bool m_requiresDehire;
        private bool m_missing;
        private int m_totalPalletsOwed;
        #endregion

        #region Form Elements

        protected RadioButtonList rdoEmailTo;

        protected Label lblNumberOfPallets;
        protected Label lblReturnReferenceNumber;

        #endregion

        private void SetVisibleDataGridColumns()
        {
            if (m_missing)
            {
                dgPCVs.Levels[0].Columns[(int)eDataGridColumns.DateOfIssue].Visible = false;
                dgPCVs.Levels[0].Columns[(int)eDataGridColumns.NoOfSignings].Visible = false;
                dgPCVs.Levels[0].Columns[(int)eDataGridColumns.PCVStatus].Visible = false;
                dgPCVs.Levels[0].Columns[(int)eDataGridColumns.PCVRedemptionStatus].Visible = false;
                dgPCVs.Levels[0].Columns[(int)eDataGridColumns.OrganisationName].Visible = false;
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            m_pcvId = 0;

            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (m_jobId > 0)
                SetJobDetails();
            else if (Request.QueryString["RequiresDehire"] != null)
                m_requiresDehire = true;
            else if (Request.QueryString["Missing"] != null)
                m_missing = true;

            if (!IsPostBack)
            {
                SetPageHeader();
                SetVisibleDataGridColumns();
                PopulateStaticControls();

                if (m_requiresDehire)
                {
                    pnlRequiresDehire.Visible = true;
                    chkPCVRedemptionStatus.ClearSelection();
                    chkPCVRedemptionStatus.Items.FindByValue(((int)ePCVRedemptionStatus.RequiresDeHire).ToString()).Selected = true;
                }
                else if (m_missing)
                {
                    SetDefaultPeriod();
                    chkPCVStatus.ClearSelection();
                    chkPCVStatus.Items.FindByValue(Utilities.UnCamelCase(ePCVStatus.Missing.ToString())).Selected = true; 
                }
                else
                    SetDefaultPeriod();

                PerformSearch();
            }
            else
                dsPCV = (DataSet)ViewState[C_PCVDATA_VS];
        }

        private void SetJobDetails()
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(m_jobId);
        }

        private void SetDefaultPeriod()
        {
            DateTime startWeek = DateTime.UtcNow;
            TimeSpan toBegin = new TimeSpan((int)startWeek.DayOfWeek, 0, 0, 0);
            startWeek = startWeek.Subtract(toBegin);

            //startWeek = startWeek.Subtract(startWeek.TimeOfDay);
            dteStartDate.SelectedDate = startWeek;

            // Add default 7 days to end date
            dteEndDate.SelectedDate = startWeek.AddDays(7);

            m_pcvId = Convert.ToInt32(Request.QueryString["pcvId"]);
        }

        private void SetPageHeader()
        {
            if (m_requiresDehire)
            {
                this.Page.Title = "PCVs Requiring Dehire  -  ";
                this.Page.Title += "A list of PCVs with a Redemption Status of Requires De-hire are displayed below.";
            }
            else if (m_missing)
            {
                this.Page.Title = "PCVs Missing  -  ";
                this.Page.Title += "A list of PCVs with a Status of Missing are displayed below.";
            }
            else if (m_jobId > 0)
            {
                this.Page.Title = "PCVs taken on Job No " + m_jobId + "  -  ";
                this.Page.Title += "A list of PCVs taken on this job is displayed below.";
            }
            else
            {
                this.Page.Title = "PCV Search  -  ";
                this.Page.Title += "Search for PCVs below.";
            }
        }

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
            this.Init += new EventHandler(ListPCV_Init);

        }
        #endregion

        #region Populate Static Controls

        ///	<summary> 
        ///	Populate Static Controls
        ///	</summary>
        private void PopulateStaticControls()
        {
            chkPCVStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(ePCVStatus)));
            chkPCVStatus.DataBind();
            
            foreach (ListItem item in chkPCVStatus.Items)
            {
                if (item.Text == Utilities.UnCamelCase(ePCVStatus.Outstanding.ToString() ))
                    item.Selected = true;
            }

            Facade.PCV facPCV = new Facade.PCV();

            chkPCVRedemptionStatus.DataSource = facPCV.GetAllPCVRedemptionStatuses(false);
            chkPCVRedemptionStatus.DataBind();

            foreach (ListItem item in chkPCVRedemptionStatus.Items)
            {
                if (item.Value == ((int)ePCVRedemptionStatus.ToBeRedeemed).ToString())
                    item.Selected = true;
            }
        }

        #endregion

        #region DBCombo's Server Methods and Initialisation

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClient.Items.Clear();
            cboClient.DataSource = boundResults;
            cboClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());

        }

        void cboClientsCustomer_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllJobRelatedFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClientsCustomer.Items.Clear();
            cboClientsCustomer.DataSource = boundResults;
            cboClientsCustomer.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region Methods & Events

        ///	<summary> 
        ///	Button Search Click
        ///	</summary>
        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            PerformSearch();
        }

        private void GetData()
        {
            Facade.IPCV facPCV = new Facade.PCV();

            int pCVId = m_pcvId;
            bool noCriteria = false;

            if ((txtVoucherNo.Text == "") && (m_jobId == 0) && (txtJobId.Text == "") && (pCVId == 0) && (cboClient.SelectedValue.ToString() == "") && (cboClientsCustomer.SelectedValue.ToString() == "") && (!dteStartDate.SelectedDate.HasValue || dteStartDate.SelectedDate.Value == DateTime.MinValue) && (!dteEndDate.SelectedDate.HasValue || dteEndDate.SelectedDate.Value == DateTime.MinValue))
                noCriteria = true;

            // CHECK TO SEE IF ANY OF THE CHECK BOX LISTS ARE CHECKED 
            foreach (ListItem item in chkPCVStatus.Items)
                if (item.Selected)
                    noCriteria = false;

            foreach (ListItem item in chkPCVRedemptionStatus.Items)
                if (item.Selected)
                    noCriteria = false;

            if (noCriteria)
            {
                // Get all PCVs
                dsPCV = facPCV.GetAll();
            }
            else
            {
                // Params
                int jobId = 0;
                try { jobId = Convert.ToInt32(txtJobId.Text); }
                catch { }

                string voucherNo = txtVoucherNo.Text;
                int clientId = cboClient.SelectedValue != "" ? Convert.ToInt32(cboClient.SelectedValue) : 0;
                int clientsCustomerId = cboClientsCustomer.SelectedValue != "" ? Convert.ToInt32(cboClientsCustomer.SelectedValue) : 0;

                string pCVStatusIdCSV = String.Empty;
                foreach (ListItem item in chkPCVStatus.Items)
                {
                    if (item.Selected)
                    {
                        if (pCVStatusIdCSV != String.Empty)
                            pCVStatusIdCSV += ",";

                        pCVStatusIdCSV += (int)Enum.Parse(typeof(ePCVStatus), item.Text.Replace(" ", "")); //Convert.ToInt32(item.Value);
                    }
                }

                string pCVRedemptionStatusIdCSV = String.Empty;

                foreach (ListItem item in chkPCVRedemptionStatus.Items)
                {
                    if (item.Selected)
                    {
                        if (pCVRedemptionStatusIdCSV != String.Empty)
                            pCVRedemptionStatusIdCSV += ",";

                        pCVRedemptionStatusIdCSV += item.Value; //(int)Enum.Parse(typeof(ePCVRedemptionStatus), item.Text.Replace(" ", ""));  //Convert.ToInt32(item); 
                    }
                }

                int hasBeenSent = cboView.Visible ? Convert.ToInt32(cboView.SelectedValue) : 0;

                DateTime startDate = dteStartDate.SelectedDate.HasValue ? dteStartDate.SelectedDate.Value : DateTime.MinValue;
                DateTime endDate = dteEndDate.SelectedDate.HasValue ? dteEndDate.SelectedDate.Value : DateTime.MinValue;
                
                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                    dsPCV = facPCV.GetwithParams(jobId, clientId, clientsCustomerId, pCVId, pCVStatusIdCSV, pCVRedemptionStatusIdCSV, hasBeenSent, voucherNo);
                else
                {
                    endDate = endDate.Add(new TimeSpan(23, 59, 00));
                    dsPCV = facPCV.GetwithParamsAndDate(jobId, clientId, clientsCustomerId, pCVId, pCVStatusIdCSV, pCVRedemptionStatusIdCSV, hasBeenSent, voucherNo, startDate, endDate);
                }
            }
        }

        private void PerformSearch()
        {
            GetData();
            for (int i = 0; i < dsPCV.Tables[0].Rows.Count; i++)
                m_totalPalletsOwed += (int)dsPCV.Tables[0].Rows[i]["NoOfPalletsOutstanding"];
            ViewState["TotalPalletsOwed"] = m_totalPalletsOwed.ToString();

            if (dsPCV.Tables[0].Rows.Count > 0)
            {
                lblError.Visible = false;
                pnlConfirmation.Visible = false;
                dgPCVs.Visible = true;
                ViewState[C_PCVDATA_VS] = dsPCV;
                dgPCVs.DataSource = dsPCV;  
                dgPCVs.CurrentPageIndex = 0;
                dgPCVs.PageSize = dsPCV.Tables[0].Rows.Count;
                dgPCVs.GroupingPageSize = dsPCV.Tables[0].Rows.Count;
                dgPCVs.DataBind();
            }
            else
            {
                //pnlConfirmation.Visible = true;
                //lblError.Visible = true;
                dgPCVs.Visible = false;
            }

            if (dsPCV.Tables[1].Rows[0][0] == DBNull.Value)
                lblPalletCount.Text = "0";
            else
                lblPalletCount.Text = dsPCV.Tables[1].Rows[0][0].ToString();
            lblPCVCount.Text = dsPCV.Tables[0].Rows.Count.ToString();

            dgPCVs.DataSource = dsPCV;
            dgPCVs.DataBind();
        }

        #endregion

        #region Active Report

        private void LoadReport()
        {
            GetData();
            Session["__ExportDS"] = dsPCV.Tables[0];
            Server.Transfer("../reports/csvexport.aspx?filename=PCVSearch.csv");
        }

        #endregion

        private void ListPCV_Init(object sender, EventArgs e)
        {
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnReport.Click += new EventHandler(btnReport_Click);
            this.btnSearch_bottom.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnReport_bottom.Click += new EventHandler(btnReport_Click);

            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClientsCustomer.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClientsCustomer_ItemsRequested);

            dgPCVs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgPCVs_ItemContentCreated);
        }

        void dgPCVs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            HtmlAnchor lnkViewPCV = (HtmlAnchor)e.Content.FindControl("lnkViewPCV");

            bool hasImage = e.Item["ScannedFormId"] != DBNull.Value && Convert.ToInt32(e.Item["ScannedFormId"]) != 0;

            if (lnkViewPCV != null)
                lnkViewPCV.Visible = hasImage;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            dgPCVs.Visible = false;
            LoadReport();
        }

        private void cfvDteEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (dteEndDate.SelectedDate.Value.ToString() != "")
            {
                if (dteStartDate.SelectedDate < dteEndDate.SelectedDate)
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
            else
                args.IsValid = true;
        }
    }
}

