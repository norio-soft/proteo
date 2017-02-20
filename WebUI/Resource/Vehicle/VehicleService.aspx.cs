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

namespace Orchestrator.WebUI.resource.vehicle
{
	/// <summary>
	/// Summary description for VehicleList.
	/// </summary>
	public partial class VehicleService : Orchestrator.Base.BasePage
	{
		#region Constants
		private const string C_VEHICLESERVICE_ID = "VehicleServiceId";
		#endregion
		
		#region Page Variables
		private int m_vehicleId = 0;
		private bool m_isUpdate = false;
		//private Entities.VehicleService vehicleService; 


        private string sourceHref
        {
            set
            {
                this.ViewState["_sourceHref"] = Request.UrlReferrer.AbsoluteUri;
            }
            get
            {
                return (string)this.ViewState["_sourceHref"];
            }
        }
		#endregion

		#region Form Elements

		protected System.Web.UI.WebControls.Button cmdAddUser;

		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);
			btnAddVehicleService.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditResource);

			if (Request.QueryString["vehicleId"] != null)
				m_vehicleId = Convert.ToInt32(Request.QueryString["vehicleId"]);

			if (!IsPostBack)
			{
				this.SortCriteria = "VehicleServiceDueDate";
				this.SortDir = "asc";
				PopulateVehicleService();
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
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
		}

        void btnCancel_Click(object sender, EventArgs e)
        {

            this.Close();
        }
		#endregion

		#region Populate Vehicle Service
		///	<summary> 
		///	Populate Vehicle Service
		///	</summary>
		private void PopulateVehicleService()
		{
			
			DataSet dsVehicleService = GetVehicleServiceData();
			dgVehicleService.DataSource = dsVehicleService;
			dgVehicleService.DataBind();
		}

		
		///	<summary> 
		///	Data Set Get Vehicle Service Data
		///	</summary
		private DataSet GetVehicleServiceData()
		{
			Facade.IVehicleService facVehicleService= new Facade.Resource();
			DataSet dsVehicleHistoryService = null;;

			dsVehicleHistoryService = facVehicleService.GetVehicleServiceHistory(m_vehicleId);
			
			return dsVehicleHistoryService ;
		}

		#endregion

		#region Vehicle Service Grid Manipulation	
		///	<summary> 
		///	Sort Dir
		///	</summary
		protected string SortDir
		{
			get {return (string)ViewState["sortDir"];}
			set { ViewState["sortDir"] = value;}
		}
    

		///	<summary> 
		///	Sort Criteria
		///	</summary
		protected string SortCriteria
		{
			get { return (string)ViewState["sortCriteria"];}
			set {ViewState["sortCriteria"] = value;}
		}


		///	<summary> 
		///	Data Grid Vehicles Service Page
		///	</summary
		protected void dgVehicleService_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgVehicleService.CurrentPageIndex = e.NewPageIndex;
			PopulateVehicleService();
		}


		
		#endregion
     
		private bool AddVehicleService()
		{
			int vehicleServiceId = 0; 
			bool retVal = false;
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

			Facade.IVehicleService facVehicleService= new Facade.Resource();
             		
			// Need to add a row in with the date of the date of the service.
            Entities.VehicleService vehicleService = new Orchestrator.Entities.VehicleService();  
			vehicleService.vehicleServiceDueDate = Convert.ToDateTime(dteNewServiceDate.SelectedDate); 
			vehicleService.vehicleId = m_vehicleId;
			vehicleServiceId = facVehicleService.Create(vehicleService, userName);
			
			dteNewServiceDate.Text = string.Empty;

			if (vehicleServiceId == 0)
			{
				lblConfirmation.Text = "There was an error adding the vehicle service, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
				retVal = false;
			}
			else
			{
				// Store the new id in the viewstate
				ViewState[C_VEHICLESERVICE_ID] = vehicleServiceId;
				retVal = true;
			} 
			
			PopulateVehicleService();
			
			return retVal;
        }

		private bool UpdateVehicleService()
		{
			// TODO: Update Vehicle ... Not within the requirements specification
			bool retVal = false;

			return retVal;
		}

		protected void btnAddVehicleService_Click(object sender, System.EventArgs e)
		{
			bool retVal = false;

			if (m_isUpdate) 
				retVal = UpdateVehicleService();
			else
				retVal = AddVehicleService();

			if (m_isUpdate) 
				lblConfirmation.Text = "The service for this vehicle has been updated successfully.";
			else
			{
				lblConfirmation.Text = "The service for this vehicle has been Added successfully.";
			

				// Switch to update mode
				//ViewState[C_VEHICLESERVICE_ID] = vehicleService.vehicleServiceId;
				m_isUpdate = true;	 
			}

			lblConfirmation.Visible = true;
		}

		
	}
}
