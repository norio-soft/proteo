using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
	/// Summary description for Invoice Breakdown.
	/// </summary>
	public partial class InvoiceBreakdown: Orchestrator.Base.BasePage
	{							  
		#region Constants & Enums
		private const int C_HOURSPREVIOUS = 4;	// Past hours to show
		private const int C_HOURSFOLLOWING = 4;	// Following hours to show
		private const int C_HOURSTOTAL = 36;	// Total hours to show
		private const int C_HOURSMOVE = 6;		// Hours to move when going back / forward
		
		private const string C_HOURS_PREVIOUS_VS = "C_HOURS_PREVIOUS_VS";
		private const string C_HOURS_FOLLOWING_VS = "C_HOURS_FOLLOWING_VS";

		private static readonly TimeSpan C_TIMESPAN = new TimeSpan(0, 0, 30, 0, 0);
		private static readonly TimeSpan C_MOVEONREFRESH = C_TIMESPAN;
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

		


		
		protected System.Web.UI.WebControls.RequiredFieldValidator	rfvClient;




		

		#endregion

		#region Page/Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

			if (Request.QueryString["IdentityId"] != null)
				m_IdentityId = Convert.ToInt32(Request.QueryString["IdentityId"]);   	

			if (Request.QueryString["SubIdentityId"] != null)
			{
				//m_Sub = true;
				m_IdentityId = Convert.ToInt32(Request.QueryString["SubIdentityId"]); 
			}

			if (!IsPostBack)
			{
				ClearFields();

				PopulateStaticControls(); 
				
				lblClient.Text = "Client";
				cboClient.Visible = true;
				//LoadGrid();
			}
		}

		protected void JobsToInvoice_Init(object sender, EventArgs e)
		{
			this.btnClear.Click +=new System.EventHandler(btnClear_Click); 
			this.btnFilter.Click +=new System.EventHandler(btnFilter_Click);
			this.dlJob.ItemDataBound += new System.Web.UI.WebControls.DataListItemEventHandler(dlJob_ItemDataBound);
			this.dlJobSub.ItemDataBound +=new DataListItemEventHandler(dlJobSub_ItemDataBound);
//			this.rdoSortType.SelectedIndexChanged +=new EventHandler(rdoSortType_SelectedIndexChanged); 
			this.rdoInvoiceType.SelectedIndexChanged +=new EventHandler(rdoInvoiceType_SelectedIndexChanged);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboSubContractor.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
		}

        

		#endregion

		#region Populate Static Controls
		private void PopulateStaticControls()
		{
			cboJobState.DataSource = Enum.GetNames(typeof(eJobState));
			cboJobState.DataBind();
			cboJobState.SelectedValue = eJobState.ReadyToInvoice.ToString(); 

			// Invoice Sort Type
			rdoSortType.DataSource =  Enum.GetNames(typeof(eInvoiceSortType));
			rdoSortType.DataBind();
			rdoSortType.Items[0].Selected = true;

			// Invoice Type
			rdoInvoiceType.Items.Add (new ListItem ("Normal Invoice/Self Bill"));
			rdoInvoiceType.Items.Add (new ListItem ("Sub Contractor"));
			//rdoInvoiceType.Items[0].Selected = true;

			if (m_IdentityId != 0)
			{
				//Get the Client name for id.
				Facade.IOrganisation facOrg = new Facade.Organisation();
				string name = facOrg.GetNameForIdentityId(m_IdentityId);
 
				cboClient.SelectedValue = m_IdentityId.ToString();
				cboClient.Text = name;
			
				// Add client specific references to sort collection
				Facade.IOrganisationReference facOrgRef = new Facade.Organisation();
				Entities.OrganisationReferenceCollection eOrf = facOrgRef.GetReferencesForOrganisationIdentityId(m_IdentityId, true);
 
				foreach( Entities.OrganisationReference ocr in eOrf )
				{
					rdoSortType.Items.Add(new ListItem (ocr.Description));
				}
			}
		}
		#endregion

		#region Combo's Server Methods and Initialisation

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

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
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text);

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
	

		
		#endregion

		#region Methods & Event Handlers
		#region Other Events
		private void rdoInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
		{
			ClearFields();
		}
		
		private void ClearFields()
		{
			if (rdoInvoiceType.SelectedIndex != -1)
			{
				pnlFilter.Visible = true;
				btnFilter.Visible = true;
				btnClear.Visible = true;
				pnlSubJob.Visible = false;
				pnlNormalJob.Visible = false;
				lblJobCount.Text  = string.Empty;
				txtJobId.Text = string.Empty;
				txtInvoiceId.Text = string.Empty;
				lblOnHold.Visible = false;

				txtClientInvoiceNumber.Text = string.Empty;
				cboClient.Text = string.Empty;
				cboClient.SelectedValue = string .Empty ;
				cboSubContractor.Text = string.Empty;
				cboSubContractor.SelectedValue = string.Empty;

				if (rdoInvoiceType.SelectedIndex == 1)
				{
					cboSubContractor.Visible = true;
					lblSubContractor.Visible = true;
					lblClientInvoiceNumber.Visible = true;
					txtClientInvoiceNumber.Visible = true;
				}
				else
				{
					cboSubContractor.Visible = false;
					lblSubContractor.Visible = false;
					lblClientInvoiceNumber.Visible = false;
					txtClientInvoiceNumber.Visible = false;
				}
			}

			lblJobState.Visible = false;
			cboJobState.Visible = false;

			// Date Fields
            dteStartDate.SelectedDate = DateTime.MinValue;
            dteEndDate.SelectedDate = DateTime.MinValue;
		}
		
		#endregion  
	
		#region Button Events
		protected void btnClear_Click(object sender, EventArgs e)
		{
			ClearFields();
		}
		
		protected void btnFilter_Click(object sender, EventArgs e)
		{
			btnFilter.DisableServerSideValidation();

			if (Page.IsValid)
			{    
				LoadGrid();
			}
		}

		#endregion

		#region Normal Jobs To Invoice

		private void dlJob_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				// References
				Facade.IJobReference facJobReference = new Facade.Job();

				LinkButton jobId = (LinkButton) e.Item.FindControl("lnkJobId");

				Repeater repReferences = (Repeater) e.Item.FindControl("repReferences");

				JobReferenceCollection jrc = facJobReference.GetJobReferences(Convert.ToInt32(jobId.Text));
				
				repReferences.DataSource = jrc;
				repReferences.DataBind();
			
				// Customers
				Facade.IJob facJobCustomer = new Facade.Job();

				Repeater repCustomers = (Repeater) e.Item.FindControl("repCustomers");
 
				DataSet ds = facJobCustomer.GetJobCustomers(Convert.ToInt32(jobId.Text)); 
				
				repCustomers.DataSource = ds;
				repCustomers.DataBind();  
			}
		}	

		private void dlJobSub_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				// References
				Facade.IJobReference facJobReference = new Facade.Job();

				LinkButton jobId = (LinkButton) e.Item.FindControl("lnkJobIdSub");

				Repeater repReferences = (Repeater) e.Item.FindControl("repReferencesSub");

				JobReferenceCollection jrc = facJobReference.GetJobReferences(Convert.ToInt32(jobId.Text));
				
				repReferences.DataSource = jrc;
				repReferences.DataBind();
			
				// Customers
				Facade.IJob facJobCustomer = new Facade.Job();

				Repeater repCustomers = (Repeater) e.Item.FindControl("repCustomersSub");
 
				DataSet ds = facJobCustomer.GetJobCustomers(Convert.ToInt32(jobId.Text)); 
				
				repCustomers.DataSource = ds;
				repCustomers.DataBind();  
			}
		}
		

		private void LoadGrid()
		{
			int clientId = 0;
			int subContractId = 0;
			int jobId = 0;
			int invoiceId = 0;
			string clientInvoiceNumber = string.Empty;

			DateTime startDate = DateTime.MinValue;
			DateTime endDate = DateTime.MinValue;  
			
			// Client
			if (cboClient.Text != "")
				clientId = Convert.ToInt32 (cboClient.SelectedValue);

			// Sub Contractor
			if (cboSubContractor.Text != "")
				subContractId = Convert.ToInt32 (cboSubContractor.SelectedValue);

			// Invoice Id
			if (txtInvoiceId.Text != "")
				invoiceId = Convert.ToInt32(txtInvoiceId.Text);

			// Client Invoice Number
			if (txtClientInvoiceNumber.Text != "")
				clientInvoiceNumber = txtClientInvoiceNumber.Text; 

//			// Job State
//			if (cboJobState.SelectedValue != string.Empty)
//				jobState = (eJobState) Enum.Parse(typeof(eJobState), cboJobState.SelectedValue);


			 // Job Id
			if (txtJobId.Text != "")
				jobId = Convert.ToInt32(txtJobId.Text);

			// Date Range
			if (dteStartDate.SelectedDate != DateTime.MinValue)
				startDate = dteStartDate.SelectedDate.Value;
			
			if (dteEndDate.SelectedDate != DateTime.MinValue)
				endDate = dteEndDate.SelectedDate.Value;

			// Get Invoiced Jobs
			Facade.IInvoice facInvoice = new Facade.Invoice();
			DataSet dsInvoicing = null; 
			
			if (rdoInvoiceType.SelectedIndex != 1)
			{
				if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
					dsInvoicing = facInvoice.GetInvoicedJobsWithParamsAndDate(clientId, subContractId, invoiceId, jobId, clientInvoiceNumber, startDate, endDate, 0);
				else
					dsInvoicing = facInvoice.GetInvoicedJobsWithParams(clientId, subContractId, invoiceId, jobId, clientInvoiceNumber, 0);
			}
			else
			{
				if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
					dsInvoicing = facInvoice.GetInvoicedJobsWithParamsAndDate(clientId, subContractId, invoiceId, jobId, clientInvoiceNumber, startDate, endDate, 1);
				else
					dsInvoicing = facInvoice.GetInvoicedJobsWithParams(clientId, subContractId, invoiceId, jobId, clientInvoiceNumber, 1);
			}

			// Check whether account is on hold
			if (rdoInvoiceType.SelectedIndex != 1)
			{
				if ( dsInvoicing.Tables[0].Rows.Count > 0)
				{
//					if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"])  == 1)
//					{
//						dlJob.Enabled = false;
//						lblOnHold.Visible = true;
//						lblOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";			
//					}
//					else
					lblOnHold.Visible = false;
			
					pnlNormalJob.Visible = true;
					pnlSubJob.Visible = false;
					dlJob.Visible = true;
					dlJob.DataSource = dsInvoicing;
					dlJob.DataBind();

					lblJobCount.Text = "There are " + dlJob.Items.Count.ToString() + " jobs invoiced.";
				}
				else
				{
					lblOnHold.Visible = true;
					lblOnHold.Text = "With the given parameters no jobs have been found.";			
					dlJob.Visible = false; 
					pnlNormalJob.Visible = false;
					pnlSubJob.Visible = false;
				}
				 
			}
			else
			{
				if ( dsInvoicing.Tables[0].Rows.Count > 0)
				{
					//					if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"])  == 1)
					//					{
					//						dlJob.Enabled = false;
					//						lblOnHold.Visible = true;
					//						lblOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";			
					//					}
					//					else
					lblOnHold.Visible = false;
			
					pnlSubJob.Visible = true;
					pnlNormalJob.Visible = false; 
					dlJobSub.Visible = true;
					dlJobSub.DataSource = dsInvoicing;
					dlJobSub.DataBind();

					lblJobCount.Text = "There are " + dlJobSub.Items.Count.ToString() + " jobs invoiced.";
				}
				else
				{
					lblOnHoldSub.Visible = true;
					lblOnHoldSub.Text = "With the given parameters no jobs have been found.";			
					dlJobSub.Visible = false; 
					pnlNormalJob.Visible = false;
					pnlSubJob.Visible = false;
				}
				 
			}

//				// Sort By
//				DataView dvInvoice = new DataView(dsInvoicing.Tables[0]);
//
//				foreach ( ListItem sortField in rdoSortType.Items )
//				{
//					if (sortField.Selected)
//						dvInvoice.Sort = sortField.Text; 
//				}
			  
				// Load List 
//			
//				pnlSort.Visible = false;
//				btnFilter.Visible = true;
//
//				btnClear.Visible = true;
		}
		
		protected void lnkJobId_Click(object sender, EventArgs e)
		{
			Response.Redirect("../Job/job.aspx?JobId="+((LinkButton) sender).Text);
		}
		
	
		protected void lnkInvoiceId_Click(object sender, EventArgs e)
		{
			Response.Redirect("../Invoicing/addupdateinvoice.aspx?InvoiceId="+((LinkButton) sender).Text);
		}

		protected void lnkClientInvoiceNumber_Click(object sender, EventArgs e)
		{
			//Response.Redirect("../addupdateinvoice.aspx?InvoiceId="+((LinkButton) sender).Text);
		}
		#endregion

		#region Date Events
		private string GetTimeDefinitions()
		{
			StringBuilder sb = new StringBuilder();

			// Configure the display range
			sb.Append("&HoursPrevious=");
			sb.Append(HoursPrevious.ToString());
			sb.Append("&HoursFollowing=");
			sb.Append(HoursFollowing.ToString());

			DateTime startDate = Convert.ToDateTime(dteStartDate.Text);
			DateTime endDate = Convert.ToDateTime (dteEndDate.Text);
			
			// Configure the search range
			// Attach the date information the url being built up
			sb.Append("&StartDate=");
			sb.Append(startDate.ToString("dd/MM/yy"));
			sb.Append("&StartTime=");
			sb.Append(startDate.ToString("HH:mm"));
			sb.Append("&EndDate=");
			sb.Append(endDate.ToString ("dd/MM/yy"));
			sb.Append("&EndTime=");
			sb.Append(endDate.ToString("HH:mm"));

			return sb.ToString();
		}

		#endregion
  	
		#region Sort Events
//		private void rdoSortType_SelectedIndexChanged(object sender, EventArgs e)
//		{	 
//			#region Sort Normal Job
//			// Save the checked jobs from previous display
//			ArrayList arrJobId = GetJobsSelected ();		
//			if (arrJobId != null)
//			{
//				foreach (object item in arrJobId )
//				{
//					if (jobIdCSV  != String.Empty)
//						jobIdCSV += ",";
//
//					jobIdCSV  += Convert.ToInt32(item);
//				}
//			}	
//
//			// Reload Grid
//			LoadGrid();
//
//			// Apply the checked jobs
//			if (jobIdCSV != null && jobIdCSV.Length > 0)
//			{
//				string[] selectedJobs = jobIdCSV.Split(',');
//
//				for (int i = 0; i < selectedJobs.Length; i++)
//				{
//					foreach (DataListItem item in dlJob.Items)
//					{
//						if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
//						{
//							// Find Job Id Compare with selected jobs
//							LinkButton jobId = (LinkButton) item.FindControl("lnkJobId");
//								
//							foreach (string iD in selectedJobs)
//							{
//								// If Id's match mark check box for include for invoice
//								if (jobId.Text == iD)
//								{
//									CheckBox chkJobId = (CheckBox) item.FindControl("chkIncludeJob");
//									chkJobId.Checked = true;
//								}
//							}
//						}
//					}
//				}
//			}
//			#endregion
//		}
//		
		#endregion
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
			this.Init += new System.EventHandler(this.JobsToInvoice_Init);

		}
		#endregion
	}
}
