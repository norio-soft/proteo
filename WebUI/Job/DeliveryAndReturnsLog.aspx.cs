using System;
using System.Collections;
using System.Collections.Specialized;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for DeliveryAndReturnsLog.
	/// </summary>
	public partial class DeliveryAndReturnsLog : Orchestrator.Base.BasePage
	{
		#region Constants

		// Number of client references to include
		private const int C_REFERENCE_COUNT_VS = 2;

		#endregion

		#region Page Variables
		private int					m_LogId = 0;
		#endregion

		#region Form Elements


		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				// Find out whether the page has a Log Id
				if (Convert.ToInt32(Request.QueryString["LogId"]) != 0)
				{
					m_LogId = Convert.ToInt32(Request.QueryString["LogId"]);  

					UploadReport(m_LogId);
				}
				else
				{
                    dteStartDate.SelectedDate = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
                    dteEndDate.SelectedDate = DateTime.Today;
				}
			}

		
		}

		protected void DeliveryAndReturnsLog_Init(object sender, System.EventArgs e)
		{
			this.btnGenerate.Click += new EventHandler(btnGenerate_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

		#endregion

		#region Active Report
		
		private void UploadReport(int LogId)
		{
			Facade.IReferenceData facRef = new Facade.ReferenceData();
			
			DataSet ds = facRef.GetLogDetails(LogId);
	
			// Apply the details to the report and html fields and load it 
			cboClient.Text = ds.Tables[0].Rows[0]["OrganisationName"].ToString();
			cboClient.SelectedValue = ds.Tables[0].Rows[0]["IdentityId"].ToString();

            dteStartDate.SelectedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["DateTimeFrom"].ToString());
			dteEndDate.SelectedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["DateTimeTo"].ToString());

			LoadReport();
		}

		private void LoadReport()
		{
			DateTime today = DateTime.UtcNow;
			today = today.Subtract(today.TimeOfDay);

			int identityId = int.Parse(cboClient.SelectedValue);

			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			Facade.IJob facJob = new Facade.Job();

			DataSet dsClientReferences = facOrganisation.GetReferencesForIdentityId(identityId);
			DataSet dsLogColumns = facOrganisation.GetIncludedLogColumnsForIdentityId(identityId);
			DataSet dsDeliveryAndReturnsLog = facJob.GetDeliveryAndReturnsLogForClient(identityId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);

			if (dsDeliveryAndReturnsLog.Tables[0].Rows.Count == 0)
			{
				lblError.Text = "No completed delivery and returns for client " + cboClient.Text + " for period " + dteStartDate.SelectedDate.Value.ToString("dd/MM/yy") + " to " + dteEndDate.SelectedDate.Value.ToString("dd/MM/yy");
				lblError.Visible = true;
				reportViewer.Visible = false;
			}

			else
			{
				lblError.Visible = false;
				// Build a comma-delimited list of column names to pass to the report from the first two
				// (by sequence number) client references and the columns, as configured in the client section.
				StringBuilder sbLogColumns = new StringBuilder();

				for (int refCounter = 0; refCounter < dsClientReferences.Tables[0].Rows.Count && refCounter < C_REFERENCE_COUNT_VS; refCounter++)
				{
					sbLogColumns.Append(Convert.ToString(dsClientReferences.Tables[0].Rows[refCounter]["Description"]) + ",");
				}

				for (int logCounter = 0; logCounter < dsLogColumns.Tables[0].Rows.Count; logCounter++)
				{
					if (logCounter == dsLogColumns.Tables[0].Rows.Count - 1)
						sbLogColumns.Append(Convert.ToString(dsLogColumns.Tables[0].Rows[logCounter]["Description"]));
					else
						sbLogColumns.Append(Convert.ToString(dsLogColumns.Tables[0].Rows[logCounter]["Description"]) + ",");
				}

				string columnNames = sbLogColumns.ToString();

				NameValueCollection reportParams = new NameValueCollection();

				reportParams.Add("Columns", columnNames);
			
				reportParams.Add("ClientId", cboClient.SelectedValue);
				reportParams.Add("Client", cboClient.Text);
				reportParams.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
				reportParams.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));

				// Configure the Session variables used to pass data to the report
				Session["LogId"] = m_LogId; 
				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DeliveryAndReturnsLog;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDeliveryAndReturnsLog;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

				if (cboClient.SelectedValue != "")
					reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
				// Show the user control
				reportViewer.Visible = true;
			}
		}

		#endregion

		#region DBCombo's Server Methods and Initialisation 

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
			this.Init += new System.EventHandler(this.DeliveryAndReturnsLog_Init);
		}
		#endregion

		#region Event Handlers

		private void btnGenerate_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid)
				LoadReport();
		}

		#endregion

		#region Validation

		protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = false;

            if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
				args.IsValid = true;
		}

		#endregion

	}
}
