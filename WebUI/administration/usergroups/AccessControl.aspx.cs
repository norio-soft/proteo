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

namespace Orchestrator.WebUI.administration.usergroups
{
	/// <summary>
	/// Summary description for AccessControl.
	/// </summary>
	public partial class AccessControl : Orchestrator.Base.BasePage
	{
		#region Form Elements

		protected HtmlInputHidden	txtSelectedRoles;

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!((Entities.CustomPrincipal)Page.User).IsInRole(((int)eUserRole.SystemAdministrator).ToString()) && !((Entities.CustomPrincipal)Page.User).IsInRole(((int)eUserRole.UserAdministrator).ToString()))
                Response.Redirect("~/security/accessdenied.aspx");
            
            if(!IsPostBack)
			{
				Facade.ISecurity facSecurity = new Facade.Security();
				
				cboSystemPortion.DataSource = facSecurity.GetAllSystemPortions();
				cboSystemPortion.DataValueField = "SystemPortionId";
				cboSystemPortion.DataTextField = "Description";
				cboSystemPortion.DataBind();
			}

		}
	
		#endregion

		#region Populate Page's Listboxes

		private void PopulateRoles()
		{
			pnlConfigureRoles.Visible = true;
			Facade.IRole facRole = new Facade.Security();
			DataSet allRoles = facRole.GetAllRoles();
			Entities.RoleCollection assignedRoles = facRole.GetRolesForSystemPortion((eSystemPortion)int.Parse(cboSystemPortion.SelectedValue));
			Entities.RoleCollection unassignedRoles = facRole.GetUnassignedRolesForSystemPortion((eSystemPortion)int.Parse(cboSystemPortion.SelectedValue));
			
			// Roles which can access system portion
			lbAssignedRoles.DataSource = assignedRoles;
			lbAssignedRoles.DataBind();

			// Roles which cannot access system portion
			lbUnassignedRoles.DataSource = unassignedRoles;
			lbUnassignedRoles.DataBind();
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
			this.Init += new System.EventHandler(this.AccessControl_Init);
		}
		#endregion

		#region Event Handlers

		private void btnAssign_Click(object sender, System.EventArgs e)
		{
			string roleDescription;
			if (lbUnassignedRoles.SelectedItem != null)
			{
				roleDescription = lbUnassignedRoles.SelectedItem.Text;
				if (lbAssignedRoles.Items.FindByText(roleDescription) == null)
				{
					lbAssignedRoles.Items.Add(lbUnassignedRoles.Items.FindByText(roleDescription));
					lbAssignedRoles.SelectedIndex = 0;
					lbUnassignedRoles.Items.Remove(lbUnassignedRoles.Items.FindByText(roleDescription));
				}
			}
		}

		private void btnGetSystemPortion_Click(object sender, System.EventArgs e)
		{
			PopulateRoles();
		}

		private void btnUnassign_Click(object sender, System.EventArgs e)
		{
			string roleDescription;
			if (lbAssignedRoles.SelectedItem != null)
			{
				roleDescription = lbAssignedRoles.SelectedItem.Text;
				if (lbUnassignedRoles.Items.FindByText(roleDescription) == null)
				{
					lbUnassignedRoles.Items.Add(lbAssignedRoles.Items.FindByText(roleDescription));
					lbUnassignedRoles.SelectedIndex = 0;
					lbAssignedRoles.Items.Remove(lbAssignedRoles.Items.FindByText(roleDescription));
				}
			}
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			Facade.IRole facRole = new Facade.Security();

			string roleIdsCSV = String.Empty;

			foreach(ListItem li in lbAssignedRoles.Items)
			{
				if (roleIdsCSV != String.Empty)
					roleIdsCSV += ",";
				roleIdsCSV += int.Parse(li.Value);
			}

			facRole.UpdateRolesForSystemPortion((eSystemPortion)int.Parse(cboSystemPortion.SelectedValue), roleIdsCSV);
		}

		protected void AccessControl_Init(object sender, System.EventArgs e)
		{
			this.btnGetSystemPortion.Click += new System.EventHandler(this.btnGetSystemPortion_Click);
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
			this.btnUnassign.Click += new System.EventHandler(this.btnUnassign_Click);
		}

		#endregion

	}
}
