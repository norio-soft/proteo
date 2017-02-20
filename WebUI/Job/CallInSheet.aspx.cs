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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for CallInSheet.
	/// </summary>
	public partial class CallInSheet : Orchestrator.Base.BasePage
	{
		#region Form Elements



		#endregion

		#region Page/Load

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

		#endregion

		private void btnReport_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				// Retrieve the report data and place it in the session variables
				GetCallInSheetData();
			}
			else
			{
				// Hide the report
				reportViewer.Visible = false;
			}
		}

		#region Populate Reports

		private DataSet GetData()
		{
			DateTime start = dteStartDate.SelectedDate.Value;

			start = start.Subtract(start.TimeOfDay);

			DateTime end = dteEndDate.SelectedDate.Value;

			end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

			int identityId = Convert.ToInt32(cboClient.SelectedValue);

			Facade.Job facJob = new Facade.Job();

			DataSet dsCallInSheet = null;
			
			dsCallInSheet = facJob.GetCallInSheetForClient(identityId, start, end, chkShowLatesOnly.Checked);

			return dsCallInSheet;
		}

		private void GetCallInSheetData()
		{
            int identityId = 0;
            bool result = Int32.TryParse(cboClient.SelectedValue, out identityId);
			
			DateTime start = dteStartDate.SelectedDate.Value;

			start = start.Subtract(start.TimeOfDay);

			DateTime end = dteEndDate.SelectedDate.Value;

			end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

			DataSet dsCallInSheet = null;

            if (result)
            {
                dsCallInSheet = GetData();

                if (dsCallInSheet.Tables[0].Rows.Count > 0)
                {
                    // Configure the report settings collection
                    lblReportError.Visible = false;
                    NameValueCollection reportParams = new NameValueCollection();
                    reportParams.Add("Client", cboClient.Text);
                    reportParams.Add("IdentityId", cboClient.SelectedValue);
                    reportParams.Add("StartDate", start.ToString("dd/MM/yy"));
                    reportParams.Add("EndDate", end.ToString("dd/MM/yy"));

                    // Configure the Session variables used to pass data to the report
                    Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.CallInSheet;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsCallInSheet;

                    if (rblOrderBy.SelectedItem.Value == "NumericLoadNo")
                        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "NumericLoadNo ASC";
                    else if (rblOrderBy.SelectedItem.Value == "BookedDateTime")
                        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "BookedDateTime ASC";
                    else if (rblOrderBy.SelectedItem.Value == "ArrivalDateTime")
                        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "ArrivalDateTime ASC";
                    else
                        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = string.Empty;

                    Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                    Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                    // Set the identity property of the user control
                    reportViewer.IdentityId = identityId;

                    // Show the user control
                    reportViewer.Visible = true;
                }
                else
                {
                    lblReportError.Text = "No Call Ins found for this client for this period.";
                    lblReportError.Visible = true;
                    reportViewer.Visible = false;
                }
            }
            else
            {
                lblReportError.Text = "Please select a client from the dropdown list.";
                lblReportError.Visible = true;
                reportViewer.Visible = false;
            }
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
			this.Init +=new EventHandler(CallInSheet_Init);
			
		}
		#endregion

		private void CallInSheet_Init(object sender, EventArgs e)
		{
			btnReport.Click += new EventHandler(btnReport_Click);
			btnExportToCSV.Click +=new EventHandler(btnExportToCSV_Click);
            btnExportDailyLog.Click +=btnExportDailyLog_Click;
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

		private void btnExportToCSV_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				int identityId = Convert.ToInt32(cboClient.SelectedValue);
			
				DateTime start = dteStartDate.SelectedDate.Value;

				start = start.Subtract(start.TimeOfDay);

				DateTime end = dteEndDate.SelectedDate.Value;

				end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

				Facade.IJob facJob = new Facade.Job();
	
				DataSet dsCallInSheet = null;

                //dsCallInSheet = facJob.GetDailyLogExportCSV(identityId, start, end);
                dsCallInSheet = facJob.GetCallInSheetForClient(identityId, start, end, chkShowLatesOnly.Checked);
				
				Session["__ExportDS"]  = dsCallInSheet.Tables[0];

				Server.Transfer("../Reports/csvexport.aspx?filename=DailyLogExport.csv");
			}
		}

        private void btnExportDailyLog_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int identityId = Convert.ToInt32(cboClient.SelectedValue);

                DateTime start = dteStartDate.SelectedDate.Value;

                start = start.Subtract(start.TimeOfDay);

                DateTime end = dteEndDate.SelectedDate.Value;

                end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

                Facade.IJob facJob = new Facade.Job();

                DataSet dsCallInSheet = null;

                dsCallInSheet = facJob.GetDailyLogExportCSV(identityId, start, end);
                //dsCallInSheet = facJob.GetCallInSheetForClient(identityId, start, end, chkShowLatesOnly.Checked);

                Session["__ExportDS"] = dsCallInSheet.Tables[0];

                Server.Transfer("../Reports/csvexport.aspx?filename=DailyLogExport.csv");
            }
        }
	}
}
