using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration.users
{
	/// <summary>
	/// Summary description for users.
	/// </summary>
	public partial class addupdateuser : Orchestrator.Base.BasePage
	{
		private int m_identityId;
		private int m_clientIdentityId;

		private bool m_isClient;

        public void addupdateuser_init(object sender, EventArgs e)
        {
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

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
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.RegisterClientUser, eSystemPortion.AddEditUser);

			Entities.User loggedOnUser;
			Entities.User updateUser;

            if (!IsPostBack)
			{
                if (Request.QueryString["rcbID"] != null)
                {
                    pnlClient.Visible = true;
                    return;
                }
                else
                {
                    if (Request.QueryString["IsClient"] != null && Request.QueryString["IsClient"].ToLower() == "true")
                    {
                        pnlTeam.Visible = false;
                        ViewState["IsClient"] = true;
                        pnlClient.Visible = true;
                        trPartTime.Visible = false;
                        m_isClient = true;
                    }

                    Facade.User facUser = new Facade.User();
                    loggedOnUser = facUser.GetUserByUserName(Context.User.Identity.Name);

                    m_identityId = Convert.ToInt32(Request.QueryString["identityId"]);
                    ViewState["identityId"] = m_identityId;
                    PopulateStaticDataFields(loggedOnUser);

                    if (m_identityId > 0)
                    // Show the update screen
                    {
                        pnlPassword.Visible = false;
                        cmdChangePassword.Visible = true;
                        btnAdd.Text = "Update";
                        cmdLock.Visible = true;

                        btnRemove.Visible = true;

                        if (m_isClient)
                        {
                            m_clientIdentityId = Convert.ToInt32(Request.QueryString["clientIdentityId"]);
                            ConfigureClientDbCombo(true);
                        }

                        // Get the User Details
                        updateUser = facUser.GetUserByIdentityId(m_identityId);
                        PopulateFields(updateUser);
                        PopulateUserGroups(updateUser);
                    }
                    else
                    // Show the add user screen
                    {
                        if (m_isClient)
                        {
                            pnlEmailDetails.Visible = true;
                        }

                        cboUserStatus.SelectedValue = "1";
                        cboUserStatus.Enabled = false;
                        cmdChangePassword.Visible = false;
                        cmdLock.Visible = false;
                        btnAdd.Text = "Add";
                        PopulateUserGroups();
                    }
                }
			}

            if (ViewState["IsClient"] != null)
            {
                m_isClient = true;
                chkCanAccessFromAnywhere.Enabled = false;
                chkCanAccessFromAnywhere.Checked = true;
            }
        }

		private void ConfigureClientDbCombo(bool isUpdate)
		{
			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			cboClient.SelectedValue = m_identityId.ToString();
			cboClient.Text = facOrganisation.GetNameForIdentityId(m_clientIdentityId);
			cboClient.Enabled = !isUpdate;
		}

		private void PopulateFields(Entities.User user)
		{
			txtUserName.Text = user.UserName;
			txtUserName.ReadOnly = true;
			txtForenames.Text = user.FirstNames;
			txtSurname.Text = user.LastName;
			cboUserStatus.Items.FindByValue(((int)user.UserStatus).ToString()).Selected = true;
			txtEmail.Text = user.Email;
            chkIsPartTime.Checked = user.IsPartTime;
			chkCanAccessFromAnywhere.Checked = user.CanAccessFromAnywhere;
            chkScannedLicense.Checked = user.HasScannerLicense;
			
			if(user.LockedOutUntil > DateTime.UtcNow)
				cmdLock.Text = "Unlock";
			else
				cmdLock.Text = "Lock";
		}

		private void PopulateStaticDataFields(Entities.User user)
		{
			// Populate the User Statuses
			Facade.IUser facUser = new Facade.User();
			DataSet dsUserStatus = facUser.GetAllUserStatus();

			cboUserStatus.DataSource = dsUserStatus;
			cboUserStatus.DataTextField="Description";
			cboUserStatus.DataValueField = "UserStatusId";
			cboUserStatus.DataBind();

			// Populate the Teams
			Facade.ReferenceData facRefData = new Facade.ReferenceData();
			DataSet dsTeam = facRefData.GetAllTeams();
			cboTeam.DataSource = dsTeam;
			cboTeam.DataTextField = "Description";
			cboTeam.DataValueField = "IdentityId";
			cboTeam.DataBind();

            string[] userRoleString = (((Entities.CustomPrincipal)Page.User).UserRole.Split(new char[] { ',' }));
            List<eUserRole> userRole = new List<eUserRole>();

            for (int i = 0; i < userRoleString.Length; i++)
               userRole.Add((eUserRole)int.Parse(userRoleString[i]));

            if (!userRole.Contains(eUserRole.SystemAdministrator))
            {
                chkCanAccessFromAnywhere.Enabled = false;
                chkScannedLicense.Enabled = false;
            }
		}

		private void PopulateUserGroups()
		{
			Facade.IReferenceData refData = new Facade.ReferenceData();
			DataSet dsRoles = refData.GetAllRoles();
			ListItem liClientRole = null;
			foreach(DataRow dr in dsRoles.Tables[0].Rows)
			{
				ListItem li = new ListItem();
				li.Value = dr["RoleId"].ToString();
				li.Text = dr["Description"].ToString();
				if (m_isClient && li.Text == "Clients")
					liClientRole = li;
				if (!m_isClient || (m_isClient && li.Text != "Clients"))
					lbAvailableRoles.Items.Add(li);
			}

			if (liClientRole != null && m_isClient)
			{
                lbSelectedRoles.Items.Add(liClientRole);
				txtSelectedRoles.Value += "," + liClientRole.Value;
			}
		}

		private void PopulateUserGroups(Entities.User user)
		{
			Facade.IReferenceData refData = new Facade.ReferenceData();
			DataSet dsRoles = refData.GetAllRoles();
			bool Selected;

			cboTeam.ClearSelection();
			ListItem teamMember = cboTeam.Items.FindByValue(user.TeamId.ToString());
			if (teamMember != null)
				teamMember.Selected = true;

			foreach(DataRow dr in dsRoles.Tables[0].Rows)
			{
				ListItem li = new ListItem(dr["Description"].ToString(), dr["RoleId"].ToString());
				Selected = false;

				string[] Roles = user.Roles.Split(',');
				foreach(string role in Roles)
				{
                    if (role == dr["RoleId"].ToString())
                        Selected = true;
				}
				if(Selected)
					lbSelectedRoles.Items.Add(li);
				else
					lbAvailableRoles.Items.Add(li);
			}
			
			string _Roles="";
            lblMessage.Text = "";

			foreach(ListItem li in lbSelectedRoles.Items)
			{
				_Roles += "," + li.Value;
                if (int.Parse(li.Value) == (int)eUserRole.Planner)
                    lblMessage.Text = "CAUTION: This user is a Planner. If you remove the Planner role, or remove this user, then all drivers assigned to this user will be unassigned.";
			}
			txtSelectedRoles.Value = _Roles;
		}

		
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
			cvSelectedRoles.ServerValidate +=new ServerValidateEventHandler(cvSelectedRoles_ServerValidate);
            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cfvClient.ServerValidate += new ServerValidateEventHandler(cfvClient_ServerValidate);
            btnAdd.Click +=new EventHandler(btnAdd_Click);
		}

        void cfvClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboClient.SelectedValue, 1, true);
        }
		#endregion

		protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.ReturnValue = "CloseAndRefresh";
            this.Close();
		}

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            bool success = false;

            Entities.CustomPrincipal loggedOnUser = (Entities.CustomPrincipal)Page.User;

            Facade.IUserAdmin facUserAdmin = new Facade.UserAdmin();
            Facade.ISecurity facSecurity = new Facade.Security();

            success = facUserAdmin.UpdateUserState((int)ViewState["identityId"], (int)eIdentityStatus.Deleted, loggedOnUser.Name);

            if (success)
            {
                this.ReturnValue = "CloseAndRefresh";
                this.Close();
            }
            else
                lblMessage.Text = "Update User failed. Please try again.";
        }

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
            addUser();
        }

		private void addUser()
		{
			if (Page.IsValid)
			{
				Entities.CustomPrincipal loggedOnUser = (Entities.CustomPrincipal) Page.User;

				Facade.IUserAdmin facUserAdmin = new Facade.UserAdmin();
				Facade.ISecurity facSecurity = new Facade.Security();

				int organisationId = 0;
				int teamId = 0;
                bool plannerRemoved = false;

				if (m_isClient == true)
				{
					organisationId = Convert.ToInt32(cboClient.SelectedValue);
				}
				else
				{
					teamId = Convert.ToInt32(cboTeam.SelectedItem.Value);
				}
				int retIdentityId;

                if (string.IsNullOrEmpty(txtSelectedRoles.Value))
                {
                    lblMessage.Text = "Edit user failed.  At least one role must be selected.";
                    return;
                }

				string[] sRoles = txtSelectedRoles.Value.Substring(1).Split(',');
				int[] iRoles = new int[sRoles.Length];

				for (int count = 0; count <= sRoles.Length - 1; count ++)
				{
					iRoles[count] = Convert.ToInt32(sRoles[count]);
				}

                var validateRolesResult = facUserAdmin.ValidateUserRoles(txtUserName.Text, iRoles);

                if (!validateRolesResult.Success)
                {
                    if (validateRolesResult.Infringements.Select(i => i.Description).Contains("PlannerRemoved") && validateRolesResult.Infringements.Count == 1)
                        plannerRemoved = true;
                    else
                    {
                        lblMessage.Text = string.Join("<br />", validateRolesResult.Infringements.Select(i => i.Description));
                        return;
                    }
                }

                if (btnAdd.Text == "Add")
				{
					// Validate password
					if(facSecurity.ValidatePassword(txtUserName.Text, txtPassword.Text))
						rfvComplex.IsValid = true;
					else
					{
						rfvComplex.IsValid = false;
						return;
					}

					if (!m_isClient)
					{
						retIdentityId = facUserAdmin.AddUser(txtUserName.Text, txtPassword.Text, txtForenames.Text, txtSurname.Text, iRoles, teamId, loggedOnUser.Name, txtEmail.Text, chkCanAccessFromAnywhere.Checked, chkScannedLicense.Checked);
					}
					else
					{
                        retIdentityId = facUserAdmin.AddUserForClient(txtUserName.Text, txtPassword.Text, txtForenames.Text, txtSurname.Text, iRoles, organisationId, loggedOnUser.Name, txtEmail.Text, chkScannedLicense.Checked);
					}

					if (retIdentityId > 0)
					{
						if (chkEmailDetails.Checked && m_isClient && pnlEmailDetails.Visible)
						{
							EmailClient();
						}
					    this.ReturnValue = "CloseAndRefresh";
                        this.Close();
					
					}
					else if (retIdentityId == -1) 
						lblMessage.Text = "The Username has already been added to the application.";
					else
						lblMessage.Text = "Add new User failed. Please try again.";
							  
				}
				else if (btnAdd.Text == "Update")
				{
					bool success = false;
					if (!m_isClient)
                        success = facUserAdmin.UpdateUser((int)ViewState["identityId"], txtPassword.Text, txtForenames.Text, txtSurname.Text, Convert.ToInt32(cboUserStatus.SelectedValue), iRoles, teamId, loggedOnUser.Name, txtEmail.Text, chkCanAccessFromAnywhere.Checked, chkScannedLicense.Checked, plannerRemoved);
					else
                        success = facUserAdmin.UpdateUserForClient((int)ViewState["identityId"], txtPassword.Text, txtForenames.Text, txtSurname.Text, Convert.ToInt32(cboUserStatus.SelectedValue), iRoles, organisationId, loggedOnUser.Name, txtEmail.Text, chkScannedLicense.Checked);

                    if (success)
                    {
                        this.ReturnValue = "CloseAndRefresh";
                        this.Close();
                    }
                    else
                        lblMessage.Text = "Update User failed. Please try again.";
				}
			}
		}

		private void EmailClient()
		{
			string emailToAddress = txtEmail.Text;

            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                Orchestrator.Globals.Configuration.MailFromName);

			mailMessage.To.Add(emailToAddress);
			mailMessage.Subject = "You have been registered for client portal access";

            var clientPortalUrl = Properties.Settings.Default.UseOldClientPortal ? Orchestrator.Globals.Configuration.OrchestratorURL : Orchestrator.Globals.Configuration.ClientPortalURL;

			mailMessage.Body =	"You have been registered for client portal access. \n\nYour username is " + txtUserName.Text +
							" and your password is " + txtPassword.Text + ".\n\nWhen logging in for the first time, you " +
                            "will be prompted to change your password.\n\nPlease access the site at " + clientPortalUrl + ".";

			mailMessage.IsBodyHtml = false;

            SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = Globals.Configuration.MailServer;
            smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                Globals.Configuration.MailPassword);

            smtp.Send(mailMessage);
            mailMessage.Dispose();
		}

		protected void cmdLock_Click(object sender, System.EventArgs e)
		{
			Entities.CustomPrincipal loggedOnUser = (Entities.CustomPrincipal)Page.User;
			Facade.UserAdmin  facUserAdmin = new Facade.UserAdmin();
			Facade.Security facSecurity = new Facade.Security();
			int IdentityId = Convert.ToInt32(Request.QueryString["identityId"]);

			if(cmdLock.Text == "Lock")
			{
				if(facUserAdmin.LockUser(IdentityId))
					cmdLock.Text = "Unlock";
				else
					lblMessage.Text = "Failed to lock User.";
			}
			else if (cmdLock.Text == "Unlock")
			{
				if(facUserAdmin.UnLockUser(IdentityId))
					cmdLock.Text = "Lock";
				else
					lblMessage.Text = "Failed to unlock User.";
			}
		}

		protected void cmdChangePassword_Click(object sender, System.EventArgs e)
		{
			if (ViewState["IsClient"] == null)
				Response.Redirect("changepassword.aspx?wiz=true&username=" + txtUserName.Text + "&identityId=" + Request["identityId"] + "&returnURL=" + this.Page.Request.Url.ToString());
			else
				Response.Redirect("changepassword.aspx?wiz=true&username=" + txtUserName.Text + "&identityId=" + Request["identityId"] + "&isClient=true");
		}

		#region DBCombo's Server Methods and Initialisation

	
		#endregion

		private void cvSelectedRoles_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = txtSelectedRoles.ToString().Length > 0;
		}
	}
}
