using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

using System.Collections.Generic;



namespace Orchestrator.WebUI.POD
{
    /// <summary>
    /// Summary description for ViewPODs.
    /// </summary>
    public partial class ViewHistoricPODs : Orchestrator.Base.BasePage
    {
        #region Enums

        public enum eDataGridColumns { PODId, ScannedDate, SignatureDate, OrganisationName, ClientsCustomer, Destination, Pickup, DeliveryDate, TrailerRef, TicketNo, JobId, LoadNo, View, Download, EmailFax, ScannedFormId };
        #endregion

        #region Constants

        private const string C_PODDATA_VS = "PODData";

        #endregion

        #region Private Fields

        private List<EF.HistoricPods> lstPODList;

        #endregion

        #region Form Elements

        protected Panel pnlClient;

        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (!this.IsPostBack)
            {
                Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.SearchForPODs);

                //Get the user's Identity row
                Facade.IUser facUser = new Facade.User();
                SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal)Page.User).UserName);
                reader.Read();

                //Is the User a Client User
                if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                {
                    //Tweak the grid headings etc. so that the are relevant for a Client user
                    ConfigureClientUI();
                }
                reader.Close();
            }

            // retrive the dataset from viewstate
            lstPODList = (List<EF.HistoricPods>)ViewState[C_PODDATA_VS];
        }

        #endregion

        private void ConfigureClientUI()
        {

            // Column names
            // Labels		
            //lblClientsCustomer.Text = "Customer";
        }
        #region DataGrid Event Handlers

        private void GetPODs()
        {
            bool noCriteria = false;

            lstPODList = null;

            if (dteDateOfScan.Text == String.Empty && dteDateOfScanFrom.Text == String.Empty && dteDateOfScanTo.Text == String.Empty &&
                    this.orderIdTextbox.Text == String.Empty && this.docketNoTextbox.Text == String.Empty)
            {
                noCriteria = true;
            }

            Facade.HistoricPod facHPOD = new Facade.HistoricPod();

            if (!noCriteria)
            {
                DateTime? scanDate, scanDateFrom, scanDateTo;

                scanDate = dteDateOfScan.SelectedDate;
                scanDateFrom = dteDateOfScanFrom.SelectedDate;
                scanDateTo = dteDateOfScanTo.SelectedDate;

                if (scanDate.HasValue)
                {
                    scanDateFrom = scanDate;
                    scanDateTo = scanDate;
                }

                if (!scanDateFrom.HasValue)
                    scanDateFrom = DateTime.MinValue;

                scanDateTo = scanDateTo.HasValue ? scanDateTo.Value.Date.AddDays(1).AddSeconds(-1) : DateTime.MaxValue;

                // Use Job Filtering
                string docketNo = this.docketNoTextbox.Text.Trim();
                int orderId = 0;

                if (!String.IsNullOrEmpty(this.orderIdTextbox.Text))
                    int.TryParse(this.orderIdTextbox.Text, out orderId);

                lstPODList = facHPOD.GetWithParams(docketNo, (orderId == 0) ? "" : orderId.ToString(), scanDateFrom, scanDateTo);
            }

            ViewState["C_PODDATA_VS"] = lstPODList;

            if ((lstPODList != null) && (lstPODList.Count > 0))
            {
                dgPODList.DataSource = lstPODList;

                dgPODList.DataBind();
                dgPODList.Visible = true;
                lblError.Visible = false;


            }
            else
            {
                lblError.Visible = true;
                dgPODList.Visible = false;
            }
        }


        #endregion

        #region Event Handlers

        private void ViewPODs_Init(object sender, System.EventArgs e)
        {
            this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
            cfvDateOfScanFrom.ServerValidate += new ServerValidateEventHandler(cfvDateOfScanFrom_ServerValidate);
        }

        private void btnFilter_Click(object sender, System.EventArgs e)
        {

            reportViewer.Visible = false;

            bool noCriteria = false;

            if (Page.IsValid)
            {
                if (dteDateOfScan.Text == String.Empty && dteDateOfScanFrom.Text == String.Empty && dteDateOfScanTo.Text == String.Empty &&
                    this.orderIdTextbox.Text == String.Empty && this.docketNoTextbox.Text == String.Empty)
                {
                    noCriteria = true;
                }

                if (!noCriteria)
                    GetPODs();
                else
                    dgPODList.Visible = false;
            }
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

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsClientUser)
                Page.MasterPageFile = "~/default_tableless_client.Master";
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new EventHandler(this.ViewPODs_Init);
        }
        #endregion

        #region ActiveReport

        private void LoadReport(string podIdsCSV)
        {

            Facade.IPOD facPOD = new Facade.POD();

            DataSet dsPODs = facPOD.GetScannedPODsForPODId(podIdsCSV);
            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.EmailFaxForm;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPODs;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            // Show the user control
            reportViewer.Visible = true;
        }

        #endregion

        private void cfvDateOfScanFrom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (dteDateOfScanFrom.SelectedDate.Value.ToString() != "")
            {
                if (dteDateOfScanFrom.SelectedDate <= dteDateOfScanTo.SelectedDate)
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
            else
                args.IsValid = true;
        }
    }
}
