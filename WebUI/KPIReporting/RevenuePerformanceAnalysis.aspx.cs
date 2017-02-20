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
using Orchestrator.WebUI.Security;

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.KPIReporting
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class RevenuePerformanceAnalysis : Orchestrator.Base.BasePage
	{
		#region Form Elements





		
		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Authorise.EnforceAuthorisation(eSystemPortion.KPI);
		}

		private void RevenuePerformanceAnalysis_Init(object sender, EventArgs e)
		{
            cfvStartDate.ServerValidate += new ServerValidateEventHandler(cfvStartDate_ServerValidate);
			btnViewReport.Click += new EventHandler(btnViewReport_Click);
		}

		#endregion

		#region Button Events

		private void btnViewReport_Click(object sender, EventArgs e)
		{
			btnViewReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
                //Get Dates for report

                DateTime startDate = dteStartDate.SelectedDate.Value.Subtract(dteStartDate.SelectedDate.Value.TimeOfDay);
                DateTime endDate = dteEndDate.SelectedDate.Value.Subtract(dteEndDate.SelectedDate.Value.TimeOfDay).Add(new TimeSpan(23, 59, 59));
                
                //Get data for the report
                Facade.IKPI facKPI = new Facade.KPI();
				DataSet ds = facKPI.GetRevenuePerformanceForPeriod(startDate, endDate);

				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RevenuePerformanceAnalysis;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

				NameValueCollection nvc = new NameValueCollection();
                nvc.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
                nvc.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));
				nvc.Add("ShowNumberOfJobs", chkShowNumberOfJobs.Checked.ToString());

				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = nvc;

				reportViewer1.Visible = true;
			}
		}

		#endregion

		#region Validation

		private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = false;

			if (rfvEndDate.IsValid)
			{
				if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
					args.IsValid = true;
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
			this.Init += new EventHandler(RevenuePerformanceAnalysis_Init);
		}
		#endregion
	}
}
