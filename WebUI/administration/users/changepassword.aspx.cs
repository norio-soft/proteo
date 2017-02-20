using System;

namespace Orchestrator.WebUI.administration.users
{
	/// <summary>
	/// Summary description for changepassword.
	/// </summary>
	public partial class changepassword : Orchestrator.Base.BasePage
	{

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditUser);

			if(!IsPostBack)
			{
                lblDisclaimer.Text = "This is a private computer system. Unauthorised access to this system is an offence under the Computer Misuse Act. Unauthorised disclosure of information on this system is an offence under the Data Protection Act.";

                if (Request.QueryString["username"] != null)
					txtUsername.Text = Request.QueryString["username"].ToString();
				
                if (Request.QueryString["isClient"] != null)
					ViewState["IsClient"] = true;
			}
		}

		#region Web Form Designer generated code

        override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			base.OnInit(e);

            btnBack.Click += new EventHandler(btnBack_Click);
		}

		#endregion

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			Facade.Security facSecurity = new Facade.Security();
			Entities.User busUser = new Entities.User();
			Entities.CustomPrincipal loggedOnUser = (Entities.CustomPrincipal)Page.User;

			if (facSecurity.ValidatePassword(txtUsername.Text, txtNewPassword.Text))
			{
				if(facSecurity.UpdatePassword(txtUsername.Text, txtNewPassword.Text, loggedOnUser.UserName))
				{
					pnlChangePassword.Visible = false;
					pnlChangePasswordConfirmation.Visible = true;
                    if (Request["returnURL"] != null)
                    {
                        Response.Redirect(Request.QueryString["returnURL"]);
                    }
				}			
				else
				{
					lblMessage.Text = ("The password has not been updated. Please note old passwords cannot be used again for at least one year.");
					lblMessage.Visible = true;
				}
			}
			else
			{
				rfvComplexPwd.IsValid = false;
				lblMessage.Visible = false;
			}
        
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			if (ViewState["IsClient"] == null)
				Response.Redirect("./userList.aspx");
			else
				Response.Redirect("./userList.aspx?isClient=true");
		}
	}
}
