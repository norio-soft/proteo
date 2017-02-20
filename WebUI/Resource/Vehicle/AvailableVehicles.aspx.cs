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

namespace Orchestrator.WebUI.Resource.Vehicle
{
	/// <summary>
	/// Summary description for AvailableVehicles.
	/// </summary>
	public partial class AvailableVehicles : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_SORT_CRITERIA_VS = "C_SORT_CRITERIA_VS";
		private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";
		private const string C_VEHICLESDS_VS = "C_DS_VEHICLES_VS";

		#endregion

		#region Page Variables

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

				PopulateVehicles();
			}
		}

		private void AvailableVehicles_Init(object sender, EventArgs e)
		{
			btnFilter.Click += new EventHandler(btnFilter_Click);

			cfvStartDate.ServerValidate += new ServerValidateEventHandler(cfvStartDate_ServerValidate);

			dgVehicles.PageIndexChanged += new DataGridPageChangedEventHandler(dgVehicles_PageIndexChanged);
			dgVehicles.SortCommand += new DataGridSortCommandEventHandler(dgVehicles_SortCommand);
		}

		#endregion

		#region Button Events

		private void btnFilter_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
				PopulateVehicles();

			dgVehicles.Visible = Page.IsValid;
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

		private void dgVehicles_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgVehicles.CurrentPageIndex = e.NewPageIndex;
			PopulateVehicles();
		}

		private void dgVehicles_SortCommand(object source, DataGridSortCommandEventArgs e)
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

			PopulateVehicles();
		}

		private void PopulateVehicles()
		{
			DataSet ds = null;

            if (dteStartDate.SelectedDate != DateTime.MinValue && dteEndDate.SelectedDate != DateTime.MinValue)
			{

                DateTime startDate = dteStartDate.SelectedDate.Value;
				startDate = startDate.Subtract(startDate.TimeOfDay);

                DateTime endDate = dteEndDate.SelectedDate.Value;
				endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));
			
				Facade.IResource facResource = new Facade.Resource();
				ds = facResource.GetAvailableForDateRange(eResourceType.Vehicle, startDate, endDate);
			}
			else
				ds = (DataSet) ViewState[C_VEHICLESDS_VS];
            
			if (ds != null)
			{
				DataView dv = new DataView(ds.Tables[0]);
				string sortExpression = SortCriteria + " " + SortDirection;
				dv.Sort = sortExpression.Trim();

				dgVehicles.DataSource = dv;
				dgVehicles.DataBind();

				ViewState[C_VEHICLESDS_VS] = ds;
			}
			else
				dgVehicles.Visible = false;
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
			this.Init += new EventHandler(AvailableVehicles_Init);
		}
		#endregion
	}
}
