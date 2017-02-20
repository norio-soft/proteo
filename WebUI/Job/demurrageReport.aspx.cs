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
using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

using System.IO;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Displays a list of jobs that have demurrage for a particular client and date-range.
	/// </summary>
	public partial class demurrageReport : Orchestrator.Base.BasePage
	{
		#region Form Elements



		

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				// Set the initial start date and time (the last week - up until midnight tonight)
				DateTime thisMoment = DateTime.UtcNow;
				DateTime startOfTomorrow = thisMoment.AddDays(1).Subtract(thisMoment.TimeOfDay);
				dteEndDate.SelectedDate = startOfTomorrow;
                dteStartDate.SelectedDate = startOfTomorrow.Subtract(new TimeSpan(8, 0, 0, 0));
			}
		}

		private void demurrageReport_Init(object sender, EventArgs e)
		{
			// Attach button event handlers
			btnReport.Click += new EventHandler(btnReport_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

		#endregion

		#region Populate Report

		private void GetDemurrageReportData()
		{
			Facade.IJob facJob = new Facade.Job();

			int identityId = int.Parse(cboClient.SelectedValue);

			DataSet dsJobsWithDemurrage = facJob.GetJobsWithDemurrage(identityId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);

			// Configure the report settings collection
			NameValueCollection reportParams = new NameValueCollection();
			reportParams.Add("Client", cboClient.Text);
			reportParams.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
			reportParams.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));

			// Configure the Session variables used to pass data to the report
			Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DemurrageReport;
			Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsJobsWithDemurrage;
			Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
			Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
			Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

			// Set the identity property of the user control
			reportViewer.IdentityId = identityId;

			// Show the user control
			reportViewer.Visible = true;
		}

		#endregion

		#region Button Event Handlers

		private void btnReport_Click(object sender, EventArgs e)
		{
			btnReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Retrieve the report data and place it in the session variables
				GetDemurrageReportData();
			}
			else
			{
				// Hide the report
				reportViewer.Visible = false;
			}
		}

		#endregion

		#region DBCombo's Server Methods and Initialisation

		void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered (e.Text);

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
			this.Init += new EventHandler(demurrageReport_Init);
		}
		#endregion
	}
}
