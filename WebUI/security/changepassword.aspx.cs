using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator;

namespace Orchestrator.WebUI.Security
{
	/// <summary>
	/// Summary description for changepassword.
	/// </summary>
    public partial class changepassword : System.Web.UI.Page
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				ViewState["returnUrl"] = Request["ReturnUrl"];
				if (Request.QueryString["username"] != null)
					txtUsername.Text = Request.QueryString["username"].ToString();
           
				if(Request["Expired"] == "true")
					lblPasswordExpired.Visible = true;
			}	
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

		}
		#endregion

		protected void btnSubmit_Click(object sender, System.EventArgs e)
		{
			Facade.Security facSecurity  = new Orchestrator.Facade.Security();
			Entities.User user = new Entities.User();		
			int authenticateResult;
			authenticateResult = facSecurity.Authenticate(txtUsername.Text, txtOldPassword.Text, 1);

			if (authenticateResult==-1 || authenticateResult==-2 || authenticateResult==-3)
			{
				lblMessage.Text = "Your old/existing password could not be authenticated.";
				lblMessage.Visible = true;
				return;
			}
			else
			{
				lblMessage.Visible = false;
			}

			if (facSecurity.ValidatePassword(txtUsername.Text, txtNewPassword.Text))
			{
				if (facSecurity.UpdatePassword(txtUsername.Text, txtNewPassword.Text, txtUsername.Text))
				{
					var logonResult = facSecurity.Logon(txtUsername.Text, txtNewPassword.Text, 1, ref user);

                    if (logonResult != Enums.eLogonResult.Success)
                        throw new ApplicationException(string.Format("Unexpected Logon result following password change: {0}", Enum.GetName(typeof(Enums.eLogonResult), logonResult)));

                    var outcome = Security.Authentication.ProcessAuthenticatedUser(txtUsername.Text, rememberMe: false, redirectOnceLoggedIn: false, customerUserSecurityKey: null);

                    if (outcome == Authentication.eProcessUserOutcome.Success || outcome == Authentication.eProcessUserOutcome.RedirectToClientPortal)
                        ViewState["redirectToClientPortal"] = outcome == Authentication.eProcessUserOutcome.RedirectToClientPortal;
                    else
                        throw new ApplicationException(string.Format("Unexpected ProcessLogon outcome following password change: {0}", Enum.GetName(typeof(Authentication.eProcessUserOutcome), outcome)));

                    pnlChangePassword.Visible = false;
					pnlChangePasswordConfirmation.Visible = true;
				}
				else
				{
					lblMessage.Text = "The password has not been updated. Please note old passwords cannot be used again for at least one year. Please try again or contact your System Administrator";
					lblMessage.Visible = true;
				}
			}
			else
			{
				rfvComplexPwd.IsValid = false;
				lblMessage.Visible = false;
			}
		}

		protected void btnContinue_Click(object sender, System.EventArgs e)
		{
            var redirectToClientPortal = (bool)ViewState["redirectToClientPortal"];

            if (redirectToClientPortal)
            {
                RenderClientPortalRedirectScript();
                return;
            }

            var returnUrl = (string)ViewState["returnUrl"];

            if (!string.IsNullOrWhiteSpace(returnUrl))
                Response.Redirect(returnUrl);
            else
                Response.Redirect("../default.aspx");
		}

        private void RenderClientPortalRedirectScript()
        {
            var script = string.Format(
                "var result = confirm('The Proteo Enterprise client portal has moved to {0}.\\n\\nPlease click OK to be redirected to the new client portal.');\nif (result) {{ window.location.replace('{0}'); }} else {{ window.location.replace('/security/login.aspx?LO=1'); }}",
                Orchestrator.Globals.Configuration.ClientPortalURL);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect_client", script, true);
        }

	}

}
