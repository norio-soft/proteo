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


namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for AvailableDrivers.
	/// </summary>
	public partial class AvailableDrivers : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_SORT_CRITERIA_VS = "C_SORT_CRITERIA_VS";
		private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";
		private const string C_DRIVERDS_VS = "C_DRIVERDS_VS";

		#endregion

		#region Form Elements





		#endregion

		#region Property Interfaces

		private string SortCriteria
		{
			get { return (string) ViewState[C_SORT_CRITERIA_VS]; }
			set { ViewState[C_SORT_CRITERIA_VS] = value; }
		}

		private string SortDirection
		{
			get { return (string) ViewState[C_SORT_DIRECTION_VS]; }
			set { ViewState[C_SORT_DIRECTION_VS] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				dteStartDate.SelectedDate = DateTime.UtcNow;
                dteEndDate.SelectedDate = dteStartDate.SelectedDate;

				PopulateDrivers();
			}
		}

		private void AvailableDrivers_Init(object sender, EventArgs e)
		{
			btnFilter.Click += new EventHandler(btnFilter_Click);

			cfvStartDate.ServerValidate += new ServerValidateEventHandler(cfvStartDate_ServerValidate);

			dgDrivers.PageIndexChanged += new DataGridPageChangedEventHandler(dgTrailers_PageIndexChanged);
			dgDrivers.SortCommand += new DataGridSortCommandEventHandler(dgTrailers_SortCommand);
		}

		#endregion

		#region Methods & Events

		private void PopulateDrivers()
		{
			DataSet ds = null;

            if (dteStartDate.SelectedDate != DateTime.MinValue && dteEndDate.SelectedDate != DateTime.MinValue)
			{
                DateTime startDate = dteStartDate.SelectedDate.Value;
				startDate = startDate.Subtract(startDate.TimeOfDay);

                DateTime endDate = dteEndDate.SelectedDate.Value;
				endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

				Facade.IResource facResource = new Facade.Resource();
				ds = facResource.GetAvailableForDateRange(eResourceType.Driver, startDate, endDate);
			}
			else
				ds = (DataSet) ViewState[C_DRIVERDS_VS];

			if (ds != null)
			{
				DataView dv = new DataView(ds.Tables[0]);
				string sortExpression = SortCriteria + " " + SortDirection;
				dv.Sort = sortExpression.Trim();

				dgDrivers.DataSource = dv;
				dgDrivers.DataBind();

				ViewState[C_DRIVERDS_VS] = ds;
			}   
			else
				dgDrivers.Visible = false;
		}
		#endregion

		#region Button Events

		private void btnFilter_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
				PopulateDrivers();

			dgDrivers.Visible = Page.IsValid;
		}

		#endregion

		#region Custom Validator

		private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
            DateTime startDate = dteStartDate.SelectedDate.Value;
			startDate = startDate.Subtract(startDate.TimeOfDay);

            DateTime endDate = dteEndDate.SelectedDate.Value;
			endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

			args.IsValid = startDate <= endDate;
		}

		#endregion

		#region DataGrid Events

		private void dgTrailers_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgDrivers.CurrentPageIndex = e.NewPageIndex;
			PopulateDrivers();
		}

		private void dgTrailers_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (SortCriteria == e.SortExpression)
			{
				if (SortDirection == "DESC")
					SortDirection = "ASC";
				else
					SortDirection = "DESC";
			}
			else
			{
				SortCriteria = e.SortExpression;
				SortDirection = "DESC";
			}

			PopulateDrivers();
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
			this.Init += new EventHandler(AvailableDrivers_Init);
		}
		#endregion
	}
}
