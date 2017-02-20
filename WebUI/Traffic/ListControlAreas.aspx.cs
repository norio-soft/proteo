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

using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.ConrtolArea
{
	/// <summary>
	/// Summary description for ListControlAreas.
	/// </summary>
	public partial class ListControlAreas : Orchestrator.Base.BasePage
	{
		private enum eControlAreaManipulation {SelectControlArea, AddControlArea, ConfigureTrafficAreas, AddTrafficArea};
		private enum eTrafficAreaManipulation {SelectTrafficArea, AddTrafficArea, ConfigurePlanners, AddPlanner};

		private const string C_CONTROL_AREA_VS = "C_CONTROL_AREA_VS";
		private const string C_TRAFFIC_AREA_VS = "C_TRAFFIC_AREA_VS";

		#region Form Elements


		// Control Area Manipulation

		// Traffic Area Manipulation

		#endregion

		#region Page Variables

		private Entities.ControlArea	m_controlArea = null;
		private Entities.TrafficArea	m_trafficArea = null;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.ControlAreaManagement);

			if (!IsPostBack)
			{
				ConfigureControlAreaManipulation(eControlAreaManipulation.SelectControlArea);
				ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.SelectTrafficArea);
			}
			else
			{
				m_controlArea = (Entities.ControlArea) ViewState[C_CONTROL_AREA_VS];
				m_trafficArea = (Entities.TrafficArea) ViewState[C_TRAFFIC_AREA_VS];
			}

			lblConfirmation.Visible = false;
		}

		private void ListControlAreas_Init(object sender, EventArgs e)
		{
			cboControlArea.SelectedIndexChanged += new EventHandler(cboControlArea_SelectedIndexChanged);
			btnAddNewControlArea.Click += new EventHandler(btnAddNewControlArea_Click);
			btnAlterControlArea.Click += new EventHandler(btnAlterControlArea_Click);

			btnAddTrafficArea.Click += new EventHandler(btnAddTrafficArea_Click);

			btnSelectTrafficArea.Click += new EventHandler(btnSelectTrafficArea_Click);

			cfvControlAreaDescription.ServerValidate += new ServerValidateEventHandler(cfvControlAreaDescription_ServerValidate);
			btnCancelControlArea.Click += new EventHandler(btnCancelControlArea_Click);
			btnActionControlArea.Click += new EventHandler(btnActionControlArea_Click);

			cboTrafficArea.SelectedIndexChanged += new EventHandler(cboTrafficArea_SelectedIndexChanged);
			btnAddNewTrafficArea.Click += new EventHandler(btnAddNewTrafficArea_Click);
			btnAlterTrafficArea.Click += new EventHandler(btnAlterTrafficArea_Click);

			btnAddUser.Click += new EventHandler(btnAddUser_Click);
			btnRemoveUser.Click += new EventHandler(btnRemoveUser_Click);

			btnSelectUser.Click += new EventHandler(btnSelectUser_Click);

			cfvTrafficAreaDescription.ServerValidate += new ServerValidateEventHandler(cfvTrafficAreaDescription_ServerValidate);
			btnCancelTrafficArea.Click += new EventHandler(btnCancelTrafficArea_Click);
			btnActionTrafficArea.Click += new EventHandler(btnActionTrafficArea_Click);
		}

		#endregion

		#region Control Area Manipulation

		private void ConfigureControlAreaManipulation(eControlAreaManipulation manipulation)
		{
			pnlConfigureControlAreasTrafficAreas.Visible = false;
			pnlAddTrafficAreaToControlArea.Visible = false;
			pnlConfigureControlArea.Visible = false;

			// Configure the manipulation panels
			switch (manipulation)
			{
				case eControlAreaManipulation.ConfigureTrafficAreas:
					pnlConfigureControlAreasTrafficAreas.Visible = true;

					cboControlAreaTrafficAreas.SelectedIndex = -1;
					using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
						cboControlAreaTrafficAreas.DataSource = facTrafficArea.GetForControlAreaId(m_controlArea.ControlAreaId);
					cboControlAreaTrafficAreas.DataTextField = "Description";
					cboControlAreaTrafficAreas.DataValueField = "TrafficAreaId";
					cboControlAreaTrafficAreas.DataBind();
					break;
				case eControlAreaManipulation.AddTrafficArea:
					pnlConfigureControlAreasTrafficAreas.Visible = true;
					pnlAddTrafficAreaToControlArea.Visible = true;

					cboAddTrafficArea.SelectedIndex = -1;
					using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
						cboAddTrafficArea.DataSource = facTrafficArea.GetAll();
					cboAddTrafficArea.DataTextField = "Description";
					cboAddTrafficArea.DataValueField = "TrafficAreaId";
					cboAddTrafficArea.DataBind();
					break;
				case eControlAreaManipulation.AddControlArea:
					pnlConfigureControlArea.Visible = true;

					if (m_controlArea != null)
					{
						txtControlAreaDescription.Text = m_controlArea.ControlAreaName;
						btnActionControlArea.Text = "Update";
					}
					else
					{
						txtControlAreaDescription.Text = String.Empty;
						btnActionControlArea.Text = "Add";
					}
					break;
				case eControlAreaManipulation.SelectControlArea:
					// Set the control area
					m_controlArea = null;
					ViewState[C_CONTROL_AREA_VS] = m_controlArea;

					// Set the drop down list
					cboControlArea.SelectedIndex = -1;
					using (Facade.IControlArea facControlArea = new Facade.Traffic())
						cboControlArea.DataSource = facControlArea.GetAll();
					cboControlArea.DataTextField = "Description";
					cboControlArea.DataValueField = "ControlAreaId";
					cboControlArea.DataBind();
					cboControlArea.Items.Insert(0, new ListItem("-- Select a Control Area -- ", "-1"));
					break;
			}
		}

		#region DropDownList Events

		/// <summary>
		/// The user has selected a control area to configure traffic areas for.
		/// </summary>
		private void cboControlArea_SelectedIndexChanged(object sender, EventArgs e)
		{
			int controlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);
			using (Facade.IControlArea facControlArea = new Facade.Traffic())
			{
				m_controlArea = facControlArea.GetForControlAreaId(controlAreaId);
				ViewState[C_CONTROL_AREA_VS] = m_controlArea;
			}

			ConfigureControlAreaManipulation(eControlAreaManipulation.ConfigureTrafficAreas);
		}

		#endregion

		#region Button Events

		private void btnAddNewControlArea_Click(object sender, EventArgs e)
		{
			m_controlArea = null;
			ViewState[C_CONTROL_AREA_VS] = m_controlArea;
			cboControlArea.SelectedIndex = -1;
			ConfigureControlAreaManipulation(eControlAreaManipulation.AddControlArea);
		}

		/// <summary>
		/// The user has selected a control area to alter - allow them to change the description and code.
		/// </summary>
		private void btnAlterControlArea_Click(object sender, EventArgs e)
		{
			btnAlterControlArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				int controlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);
				using (Facade.IControlArea facControlArea = new Facade.Traffic())
				{
					m_controlArea = facControlArea.GetForControlAreaId(controlAreaId);
					ViewState[C_CONTROL_AREA_VS] = m_controlArea;
				}

				ConfigureControlAreaManipulation(eControlAreaManipulation.AddControlArea);
			}
		}

		private void btnAddTrafficArea_Click(object sender, EventArgs e)
		{
			ConfigureControlAreaManipulation(eControlAreaManipulation.AddTrafficArea);
		}

		private void btnSelectTrafficArea_Click(object sender, EventArgs e)
		{
			btnSelectTrafficArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Add this traffic area to this control area.
				int trafficAreaId = Convert.ToInt32(cboAddTrafficArea.SelectedValue);
				bool success = false;

				using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
				{
					Entities.TrafficArea trafficArea = facTrafficArea.GetForTrafficAreaId(trafficAreaId);
					trafficArea.ControlAreaId = m_controlArea.ControlAreaId;

					success = facTrafficArea.Update(trafficArea, ((Entities.CustomPrincipal) Page.User).UserName);
				}

				if (success)
				{
					lblConfirmation.Text = "The traffic area has been added.";
					ConfigureControlAreaManipulation(eControlAreaManipulation.ConfigureTrafficAreas);
				}
				else
					lblConfirmation.Text = "The traffic area has not been added.";
				lblConfirmation.Visible = true;
			}
		}
		
		private void btnCancelControlArea_Click(object sender, EventArgs e)
		{
			ConfigureControlAreaManipulation(eControlAreaManipulation.SelectControlArea);
		}

		private void btnActionControlArea_Click(object sender, EventArgs e)
		{
			btnActionControlArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				if (m_controlArea == null)
					m_controlArea = new Entities.ControlArea();
				m_controlArea.ControlAreaName = txtControlAreaDescription.Text;

				bool success = false;
				using (Facade.IControlArea facControlArea = new Facade.Traffic())
				{
					if (m_controlArea.ControlAreaId == 0)
					{
						m_controlArea.ControlAreaId = facControlArea.Create(m_controlArea, ((Entities.CustomPrincipal) Page.User).UserName);
						success = m_controlArea.ControlAreaId > 0;
					}
					else
						success = facControlArea.Update(m_controlArea, ((Entities.CustomPrincipal) Page.User).UserName);
				}

				if (success)
				{
					lblConfirmation.Text = "The control area has been stored.";
					ConfigureControlAreaManipulation(eControlAreaManipulation.SelectControlArea);
				}
				else
					lblConfirmation.Text = "The control area has not been stored.";
				lblConfirmation.Visible = true;
			}
		}

		#endregion

		#region Validation Events

		/// <summary>
		/// The control area description must be unique.
		/// </summary>
		private void cfvControlAreaDescription_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;

			foreach (ListItem item in cboControlArea.Items)
				if (item.Text.ToLower() == args.Value.ToLower())
					if (m_controlArea == null || item.Value != m_controlArea.ControlAreaId.ToString())
						args.IsValid = false;
		}

		#endregion

		#endregion

		#region Traffic Area Manipulation

		private void ConfigureTrafficAreaManupulation(eTrafficAreaManipulation manipulation)
		{
			pnlConfigureTrafficAreaUsers.Visible = false;
			pnlAddUserToTrafficArea.Visible = false;
			pnlConfigureTrafficArea.Visible = false;

			// Configure the manipulation panels
			switch (manipulation)
			{
				case eTrafficAreaManipulation.ConfigurePlanners:
					pnlConfigureTrafficAreaUsers.Visible = true;
					
					cboTrafficAreaUsers.SelectedIndex = -1;
					using (Facade.IUser facUser = new Facade.User())
						cboTrafficAreaUsers.DataSource = facUser.GetUsersForTrafficAreaId(m_trafficArea.TrafficAreaId);
					cboTrafficAreaUsers.DataTextField = "FullName";
					cboTrafficAreaUsers.DataValueField = "IdentityId";
					cboTrafficAreaUsers.DataBind();
					break;
				case eTrafficAreaManipulation.AddPlanner:
					pnlConfigureTrafficAreaUsers.Visible = true;
					pnlAddUserToTrafficArea.Visible = true;

					cboAddPlanner.SelectedIndex = -1;
					using (Facade.IUser facUser = new Facade.User())
						cboAddPlanner.DataSource = facUser.GetAllUsersInRole(eUserRole.Planner);
					cboAddPlanner.DataTextField = "FullName";
					cboAddPlanner.DataValueField = "IdentityId";
					cboAddPlanner.DataBind();
					break;
				case eTrafficAreaManipulation.AddTrafficArea:
					pnlConfigureTrafficArea.Visible = true;

					cboControlAreaForNewTrafficArea.ClearSelection();
					using (Facade.IControlArea facControlArea = new Facade.Traffic())
						cboControlAreaForNewTrafficArea.DataSource = facControlArea.GetAll();
					cboControlAreaForNewTrafficArea.DataTextField = "Description";
					cboControlAreaForNewTrafficArea.DataValueField = "ControlAreaId";
					cboControlAreaForNewTrafficArea.DataBind();

					if (m_trafficArea != null)
					{
						txtTrafficAreaDescription.Text = m_trafficArea.TrafficAreaName;
						cboControlAreaForNewTrafficArea.Items.FindByValue(m_trafficArea.ControlAreaId.ToString()).Selected = true;
						btnActionTrafficArea.Text = "Update";
					}
					else
					{
						txtTrafficAreaDescription.Text = String.Empty;
						btnActionTrafficArea.Text = "Add";
					}
					break;
				case eTrafficAreaManipulation.SelectTrafficArea:
					// Set the drop down list
					cboTrafficArea.SelectedIndex = -1;
					using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
						cboTrafficArea.DataSource = facTrafficArea.GetAll();
					cboTrafficArea.DataTextField = "Description";
					cboTrafficArea.DataValueField = "TrafficAreaId";
					cboTrafficArea.DataBind();
					cboTrafficArea.Items.Insert(0, new ListItem("-- Select a Traffic Area -- ", "-1"));
					break;
			}
		}
		
		#region DropDownList Events

		private void cboTrafficArea_SelectedIndexChanged(object sender, EventArgs e)
		{
			int trafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);
			using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
			{
				m_trafficArea = facTrafficArea.GetForTrafficAreaId(trafficAreaId);
				ViewState[C_TRAFFIC_AREA_VS] = m_trafficArea;
			}
			ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.ConfigurePlanners);
		}

		#endregion

		#region Button Events

		private void btnAddNewTrafficArea_Click(object sender, EventArgs e)
		{
			m_trafficArea = null;
			ViewState[C_TRAFFIC_AREA_VS] = m_trafficArea;
			cboTrafficArea.ClearSelection();
			ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.AddTrafficArea);
		}

		private void btnAlterTrafficArea_Click(object sender, EventArgs e)
		{
			btnAlterTrafficArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				int trafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);
				using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
				{
					m_trafficArea = facTrafficArea.GetForTrafficAreaId(trafficAreaId);
					ViewState[C_TRAFFIC_AREA_VS] = m_trafficArea;
				}

				ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.AddTrafficArea);
			}
		}

		private void btnAddUser_Click(object sender, EventArgs e)
		{
			ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.AddPlanner);
		}

		private void btnRemoveUser_Click(object sender, EventArgs e)
		{
			btnRemoveUser.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Remove this planner from this traffic area.
				int identityId = Convert.ToInt32(cboTrafficAreaUsers.SelectedValue);
				bool success = false;

				using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
					success = facTrafficArea.DeleteUser(m_trafficArea.TrafficAreaId, identityId, ((Entities.CustomPrincipal) Page.User).UserName);

				if (success)
				{
					lblConfirmation.Text = "The planner is no longer attached to the traffic area.";
					ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.ConfigurePlanners);
				}
				else
					lblConfirmation.Text = "The planner could not be removed from the traffic area.";

				lblConfirmation.Visible = true;
			}
		}

		private void btnSelectUser_Click(object sender, EventArgs e)
		{
			btnSelectTrafficArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Add this planner to this traffic area.
				int identityId = Convert.ToInt32(cboAddPlanner.SelectedValue);
				bool success = false;

				using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
					success = facTrafficArea.CreateUser(m_trafficArea, identityId, ((Entities.CustomPrincipal) Page.User).UserName);

				if (success)
				{
					lblConfirmation.Text = "The planner is now attached to the traffic area.";
					ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.ConfigurePlanners);
				}
				else
					lblConfirmation.Text = "The planner could not be attached to the traffic area.";

				lblConfirmation.Visible = true;
			}
		}

		private void btnCancelTrafficArea_Click(object sender, EventArgs e)
		{
			ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.SelectTrafficArea);
		}

		private void btnActionTrafficArea_Click(object sender, EventArgs e)
		{
			btnActionTrafficArea.DisableServerSideValidation();

			if (Page.IsValid)
			{
				if (m_trafficArea == null)
					m_trafficArea = new Entities.TrafficArea();
				m_trafficArea.TrafficAreaName = txtTrafficAreaDescription.Text;
				m_trafficArea.ControlAreaId = Convert.ToInt32(cboControlAreaForNewTrafficArea.SelectedValue);

				bool success = false;
				using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
				{
					if (m_trafficArea.TrafficAreaId == 0)
					{
						m_trafficArea.TrafficAreaId = facTrafficArea.Create(m_trafficArea, ((Entities.CustomPrincipal) Page.User).UserName);
						success = m_trafficArea.TrafficAreaId > 0;
					}
					else
						success = facTrafficArea.Update(m_trafficArea, ((Entities.CustomPrincipal) Page.User).UserName);
				}

				if (success)
				{
					lblConfirmation.Text = "The traffic area has been stored.";
					ConfigureTrafficAreaManupulation(eTrafficAreaManipulation.SelectTrafficArea);
				}
				else
					lblConfirmation.Text = "The traffic area has not been stored.";
				lblConfirmation.Visible = true;
			}
		}
		
		#endregion

		#region Validation Events

		/// <summary>
		/// The traffic area description must be unique.
		/// </summary>
		private void cfvTrafficAreaDescription_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;

			foreach (ListItem item in cboTrafficArea.Items)
				if (item.Text.ToLower() == args.Value.ToLower())
					if (m_trafficArea == null || item.Value != m_trafficArea.TrafficAreaId.ToString())
					args.IsValid = false;
		}

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
			this.Init += new EventHandler(ListControlAreas_Init);
		}
		#endregion
	}
}
