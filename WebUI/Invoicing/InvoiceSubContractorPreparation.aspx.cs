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

using Orchestrator.Entities;
using Orchestrator.Globals;
using Orchestrator.WebUI.Security;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for InvoiceSubContractorPreparation.
	/// </summary>
	public partial class InvoiceSubContractorPreparation : Orchestrator.Base.BasePage
	{
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

		#region Constants & Enums
		private const int C_HOURSPREVIOUS = 4;	// Past hours to show
		private const int C_HOURSFOLLOWING = 4;	// Following hours to show
		private const int C_HOURSTOTAL = 36;	// Total hours to show
		private const int C_HOURSMOVE = 6;		// Hours to move when going back / forward
		
		private const string C_HOURS_PREVIOUS_VS = "C_HOURS_PREVIOUS_VS";
		private const string C_HOURS_FOLLOWING_VS = "C_HOURS_FOLLOWING_VS";

		private static readonly TimeSpan C_TIMESPAN = new TimeSpan(0, 0, 30, 0, 0);
		private static readonly TimeSpan C_MOVEONREFRESH = C_TIMESPAN;

        private const string C_EXPORTCSV_VS = "ExportCSV";
		#endregion

		#region Page Variables
		private int							m_IdentityId = 0;
		private string						jobIdCSV = String.Empty;
		#endregion

		#region Property Interfaces
		private int HoursPrevious
		{
			get
			{
				if (ViewState[C_HOURS_PREVIOUS_VS] == null)
				{
					HoursPrevious = 6;
					return 6;
				}
				else
					return (int) ViewState[C_HOURS_PREVIOUS_VS];
			}
			set
			{
				ViewState[C_HOURS_PREVIOUS_VS] = value;
			}
		}

		private int HoursFollowing
		{
			get
			{
				if (ViewState[C_HOURS_FOLLOWING_VS] == null)
				{
					HoursFollowing = 3;
					return 3;
				}
				else
					return (int) ViewState[C_HOURS_FOLLOWING_VS];
			}
			set
			{
				ViewState[C_HOURS_FOLLOWING_VS] = value;
			}
		}

		#endregion

		#region Form Elements
		protected System.Web.UI.WebControls.RequiredFieldValidator		rfvClient;
		#endregion

		#region Page Load/Init
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);
            if (Request.QueryString["rcbID"] != null) return;

            if (Request.QueryString["SubContractorIdentityId"] != null)
                m_IdentityId = Convert.ToInt32(Request.QueryString["SubContractorIdentityId"]); 
			
            if (!IsPostBack)
			{
                Utilities.ClearInvoiceSession();
				PopulateStaticControls(); 
			}
		}

        protected void InvoiceSubContractorPreparation_Init(object sender, EventArgs e)
        {
            this.btnExport.Click +=new EventHandler(btnExport_Click);
            
            this.cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            this.cboSubContractor.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboSubContractor_SelectedIndexChanged);

            this.btnCreate.Click += new EventHandler(btnCreate_Click);
            this.btnFilter.Click += new EventHandler(btnFilter_Click);
            this.btnAssign.Click +=new EventHandler(btnAssign_Click);
            this.btnClear.Click += new EventHandler(btnClear_Click);

            this.btnCreate1.Click += new EventHandler(btnCreate_Click);
            this.btnFilter1.Click += new EventHandler(btnFilter_Click);
            this.btnClear1.Click += new EventHandler(btnClear_Click);

            dvJobs.RowDataBound += new GridViewRowEventHandler(dvJobs_RowDataBound);
        }

        void dvJobs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                HtmlAnchor lnkRate = (HtmlAnchor)e.Row.FindControl("lnkRate");
                int jobId = (int) ((DataRowView) e.Row.DataItem)["JobId"];
                DateTime lastUpdate = (DateTime) ((DataRowView) e.Row.DataItem)["LastUpdateDate"];

                chkSelect.Attributes.Add("onClick", "javascript:GetCheckedItems('" + jobId.ToString() + "', '" + lnkRate.ClientID + "', this);");

                lnkRate.HRef = "javascript:openSubContractWindow('" + jobId.ToString() + "', '" + lastUpdate.ToString() + "', '" + chkSelect.ClientID + "', '" + lnkRate.ClientID + "');";
            }
        }


        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //  Get data set and then put in session
                DataView dvExport = new DataView();

                dvExport = (DataView)Session[C_EXPORTCSV_VS];

                Session["__ExportDS"] = dvExport.Table;

                Server.Transfer("../Reports/csvexport.aspx?filename=InvoiceSubContractPreparation.csv");
            }
        }

        void btnClear_Click(object sender, EventArgs e)
        {
            // Configure the sub contractor
            cboSubContractor.Text = string.Empty;
            cboSubContractor.SelectedValue = string.Empty;

            // Configure the hidden variables
            hidSelectedJobs.Value = string.Empty;
            hidJobCount.Value = "0";
            hidJobTotal.Value = "0"; 
        }

        #endregion

		#region Populate Static Controls
		private void PopulateStaticControls()
		{
			if (m_IdentityId != 0)
			{
				//Get the Client name for id.
				Facade.IOrganisation facOrg = new Facade.Organisation();
				string name = facOrg.GetNameForIdentityId(m_IdentityId);
 
				cboSubContractor.SelectedValue = m_IdentityId.ToString();
				cboSubContractor.Text = name;
                 
				pnlFilter.Visible = true;
			}
		}
		#endregion

		#region Methods & Functions
	
		#region Other
		private void LoadGrid()
		{
			int clientId = 0;
			DateTime startDate = DateTime.MinValue;
			DateTime endDate = DateTime.MinValue;  

			// Client
			if (cboSubContractor.Text != "")
				clientId = Convert.ToInt32(cboSubContractor.SelectedValue);

			// Date Range
            if (dteStartDate.SelectedDate != DateTime.MinValue)
				startDate = dteStartDate.SelectedDate.Value;

            if (dteEndDate.SelectedDate != DateTime.MinValue)
				endDate = dteEndDate.SelectedDate.Value;

			bool posted = false;
			
			// Get Jobs to Invoice
			Facade.IInvoiceSubContrator facInvoice = new Facade.Invoice();
			DataSet dsInvoicing; 
			
			if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
				dsInvoicing = facInvoice.GetSubContractorJobswithParamsAndDate(clientId, 0, posted, startDate, endDate);
			else
				dsInvoicing = facInvoice.GetSubContractorJobswithParams(clientId, 0, posted);
			
			// Load List 
            dvJobs.DataSource = dsInvoicing;
           
            DataView invoicableJobs = new DataView(dsInvoicing.Tables[0]);
            Session[C_EXPORTCSV_VS] = invoicableJobs.Table;

            // Bind jobs to datagrid 
            dvJobs.DataBind();
            lblJobCount.Text = "There are " + dsInvoicing.Tables[0].Rows.Count.ToString() + " sub contractors jobs ready to invoice.";

			pnlSubContractor.Visible = true; 
            btnFilter.Visible = true;
            btnFilter1.Visible = true;
			btnClear.Visible = true;
            btnClear1.Visible = true;
			btnAssign.Visible  = cboSubContractor.SelectedValue != "";
			lblInvoiceNumber.Visible = cboSubContractor.SelectedValue != "";
			txtInvoiceNumber.Visible = cboSubContractor.SelectedValue != "";
		}

		protected ArrayList GetJobsSelected ()
		{
            string working = hidSelectedJobs.Value;
            ArrayList retval = new ArrayList();

            if (working.Length > 0)
                working = working.Substring(0, working.Length - 1);

            string[] jobIds = working.Split(',');

            foreach (string jobId in jobIds)
                retval.Add(jobId);

            return retval;
		}

		#endregion
	
		#region Button Events
		private void btnCreate_Click(object sender, EventArgs e)
		{
			btnCreate.DisableServerSideValidation();
            btnCreate1.DisableServerSideValidation();

			ArrayList arrJobId = null;
		
			arrJobId = GetJobsSelected(); 

			if (arrJobId.Count != 0)
			{
                //#15867 J.Steele 
                //Clear the Invoice Session variables before setting the specific ones
                Utilities.ClearInvoiceSession();

				Session["StartDate"] = dteStartDate.SelectedDate.Value.ToString();
				Session["EndDate"] = dteEndDate.SelectedDate.Value.ToString();
				Session["JobIds"] = arrJobId;
				
				Session["ClientId"] = Convert.ToInt32 (cboSubContractor.SelectedValue); 
				Session["ClientName"] = cboSubContractor.Text;
				Server.Transfer("addupdatesubcontractorinvoice.aspx");
			}
		}

		private void btnAssign_Click (object sender, EventArgs e)
		{
			ArrayList arrJobId = null;
			string jobIdCSV = string.Empty;
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

			btnAssign.DisableServerSideValidation();

            arrJobId = GetJobsSelected();

            if (arrJobId.Count == 0)
            {
                foreach (object item in arrJobId)
                {
                    if (jobIdCSV != String.Empty)
                        jobIdCSV += ",";

                    jobIdCSV += (item);
                }

                lblAssignUpdate.Text = "No jobs have be selected to assign invoice number, please select jobs to assign.";
                lblAssignUpdate.ForeColor = Color.Red;
                return;
            }
            else
            {
                // Add the invoice number to the selected jobs ids.
                Facade.IJobSubContractor facSub = new Facade.Job();

                Entities.FacadeResult result = facSub.AssignInvoiceNumber(txtInvoiceNumber.Text, hidSelectedJobs.Value, userName); //brInvoice.ValidateSubContractors(jobIdCSV);

                if (result.Success)
                {
                    lblAssignUpdate.Text = "The jobs have been assigned the customers invoice number";
                    lblAssignUpdate.ForeColor = Color.Blue;
                    txtInvoiceNumber.Text = string.Empty;

                    LoadGrid();
                }
                else
                {
                    // Display errors
                    infringementDisplay.Infringements = result.Infringements;
                    infringementDisplay.DisplayInfringments();

                    lblAssignUpdate.Text = "The jobs have been failed to assign the customers invoice number";
                    lblAssignUpdate.ForeColor = Color.Red;
                }
            }
		}

		private void btnFilter_Click(object sender, EventArgs e)
		{
            btnFilter.DisableServerSideValidation();
            btnFilter1.DisableServerSideValidation();
            
            if (Page.IsValid)
            {
                hidSelectedJobs.Value = String.Empty;
                hidJobCount.Value = "0";
                hidJobTotal.Value = "0";

                LoadGrid();
            }
		}
		#endregion

		#endregion 

		#region Sub Contractor Invoice Grid

		#endregion 

		#region DBCombo's Server Methods and Initialisation
        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.SubContractor);

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
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboSubContractor_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            m_IdentityId = Convert.ToInt32(cboSubContractor.SelectedValue);
            LoadGrid();
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
			this.Init += new System.EventHandler(this.InvoiceSubContractorPreparation_Init);

		}
		#endregion
	}
}
