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
using System.Diagnostics;

namespace Orchestrator.WebUI.Resource.Vehicle
{
	/// <summary>
	/// Summary description for VehicleRequiringService.
	/// </summary>
	public partial class VehicleRequiringService : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_SORT_CRITERIA_VS = "C_SORT_CRITERIA_VS";
		private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";

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

		#region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				// Default MOT Expiry seven days from now
                
                rdiMOTDueDate.SelectedDate = DateTime.Now.AddDays(7);
				PopulateVehicles();
			}
		}

		private void VehicleRequiringService_Init(object sender, EventArgs e)
		{
       		this.btnGetVehicles.Click += new System.EventHandler(this.btnGetVehicles_Click);
		}
       
		#endregion

		#region Populate and Display Controls/Elements

		private void PopulateVehicles()
		{
			Facade.IVehicle facResource = new Facade.Resource();
            DateTime dte = rdiMOTDueDate.SelectedDate.Value.AddMonths(1).AddDays(-1);
            
			DataSet dsVehicles = facResource.GetVehicleForMOTExpiry(dte);

			if (dsVehicles != null)
			{
				dgVehicles.DataSource = dsVehicles;
				dgVehicles.DataBind();
			}
			else
				dgVehicles.Visible = false;
		}

		#endregion

		#region Methods & Events
		private void btnGetVehicles_Click(object sender, System.EventArgs e) 
		{
			PopulateVehicles();
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
			this.Init +=new EventHandler(VehicleRequiringService_Init);

		}
		#endregion
	}
}
