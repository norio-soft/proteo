using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for WeekendSheet.
	/// </summary>
	public partial class WeekendSheet : Orchestrator.Base.BasePage
	{
		#region Form Elements

		
		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				PopulateStaticControls();

			lblError.Visible = false;
		}

		#endregion

		#region Populate Static Controls

		private void PopulateStaticControls()
		{
            using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
                chkTrafficArea.DataSource = facTrafficArea.GetAll();

            chkTrafficArea.DataBind();

            DateTime dteStart = NextDate(DayOfWeek.Saturday);
			dteStartDate.SelectedDate = dteStart;
            dteEndDate.SelectedDate = NextDate(DayOfWeek.Monday, dteStart);
		}

		#endregion

		#region Helper Method

        public DateTime NextDate(DayOfWeek dayOfWeek)
        {
            return NextDate(dayOfWeek, DateTime.UtcNow);
        }

		public DateTime NextDate(DayOfWeek dayOfWeek, DateTime dayFrom)
		{
            DateTime baseDate = dayFrom;

			while (baseDate.DayOfWeek != dayOfWeek)
				baseDate = baseDate.AddDays(1);

			return baseDate;
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
			btnReport.Click +=new EventHandler(btnReport_Click);
		}
		#endregion

		#region Event Handlers

		private void btnReport_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
				LoadWeekendSheet();
		}

		#endregion

		#region Load ActiveReport

		private void LoadWeekendSheet()
		{
			DateTime startDate = dteStartDate.SelectedDate.Value;
			startDate = startDate.Subtract(startDate.TimeOfDay);

			DateTime endDate = dteEndDate.SelectedDate.Value;
			endDate = endDate.Subtract(endDate.TimeOfDay);
			endDate = endDate.Add(new TimeSpan(0, 23, 59, 59));

			string organisationLocationIds = string.Empty;
			foreach (ListItem item in chkTrafficArea.Items)
				if (item.Selected)
				{
					if (organisationLocationIds.Length > 0)
						organisationLocationIds += ",";
					organisationLocationIds += item.Value;
				}

			DataSet dsWeekendSheet = null;

			using (Facade.IDriver facDriver = new Facade.Resource())
				dsWeekendSheet = facDriver.GetWeekendSheet(organisationLocationIds, startDate, endDate);

			if (dsWeekendSheet != null && dsWeekendSheet.Tables[0].Rows.Count > 0)
			{
				// Configure the Session variables used to pass data to the report
				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.WeekendSheet;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsWeekendSheet;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

				// Set the identity property of the user control
				reportViewer.IdentityId = Configuration.IdentityId;

				// Show the user control
				reportViewer.Visible = true;
			}
			else
				lblError.Text = "No results!";

			lblError.Visible = !reportViewer.Visible;
		}

		#endregion

	}
}
