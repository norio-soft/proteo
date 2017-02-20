using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator;
using Orchestrator.Globals;
using Orchestrator.Logging;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Security
{
    /// <summary>
    /// Summary description for login.
    /// </summary>
    public partial class login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Request.QueryString["LO"] == "1" && ViewState["HasLoggedOff"] == null)
            {
                // Handle the log off request.
                this.LogOff(true);
                RevokeDragAndDropPlanningAuthCookie();
            }
            else if (Request.QueryString["CPR"] == "1")
            {
                // CPR query string indicates a client portal redirect - a client user has attempted to access a page from the old client portal.
                this.LogOff(false, false);
                this.RenderClientPortalRedirectScript();
            }
            else
            {
                RevokeDragAndDropPlanningAuthCookie();

                if (!(Request.QueryString["t"] == null) && Request.QueryString["t"] == "1")
                {
                    // Handle the session time out.
                    lblMessage.Text = "Your session has timed out. Please login again.";
                    lblMessage.Visible = true;
                }

                if (!IsPostBack)
                {
                    if (Request.QueryString["HasLoggedOff"] == "1")
                    {
                        if (Request.QueryString["multiples"] == "1")
                        {
                            lblMessage.Text = "Your session has been automatically closed because you have logged in on another browser or PC.";
                            lblMessage.Visible = true;
                        }
                        if (Request.QueryString["lockordeleted"] == "2")
                        {
                            lblMessage.Text = "Your session has been automatically closed because your account has been deleted. Please contact your local administrator.";
                            lblMessage.Visible = true;
                        }
                        if (Request.QueryString["lockordeleted"] == "1")
                        {
                            lblMessage.Text = "Your session has been automatically closed because you have been locked out. Please contact your local administrator.";
                            lblMessage.Visible = true;
                        }
                    }
                    else
                    {
                        // Check for a remember me cookie and bypass the login process.
                        var username = Security.Authentication.GetRememberMeUsername();

                        if (!string.IsNullOrWhiteSpace(username))
                            this.ProcessLogon(username, rememberMe: true, redirectOnceLoggedIn: true);
                    }
                }
            }
        }

        protected void btnLogon_Click(object sender, System.EventArgs e)
        {
            int logonAttempts = 1;

            if (ViewState["LogonAttempts"] != null)
                logonAttempts = Convert.ToInt32(ViewState["LogonAttempts"]);

            Entities.User user = null;
            var facSecurity = new Facade.Security();
            var logonResult = facSecurity.Logon(txtUserName.Text, txtPIN.Text, logonAttempts, ref user);

            switch (logonResult)
            {
                case Enums.eLogonResult.Success:
                    // Check that this user can log in from this location.
                    bool canProceed = facSecurity.CanLoginFromLocation(txtUserName.Text, Request.ServerVariables["REMOTE_ADDR"]);

                    if (canProceed)
                    {
                        switch (user.UserStatus)
                        {
                            case Enums.eUserStatus.FirstLogon:
                                GoToChangePassword();
                                break;
                            case Enums.eUserStatus.PasswordReset:
                                GoToChangePassword();
                                break;
                            case Enums.eUserStatus.Active:
                                this.ProcessLogon();
                                break;
                        }
                        Session["UserName"] = user.UserName;
                    }
                    else
                        Response.Redirect("illegallocation.aspx");
                    break;
                case Enums.eLogonResult.UsernameInvalid:
                    lblMessage.Text = "Invalid Username.";
                    lblMessage.Visible = true;
                    break;
                case Enums.eLogonResult.AccountLocked:
                    lblMessage.Text = "This account has been locked. Please contact your local/site administrator.";
                    lblMessage.Visible = true;
                    break;
                case Enums.eLogonResult.PasswordInvalid:
                    lblMessage.Text = "Incorrect Password.";
                    lblMessage.Visible = true;
                    logonAttempts++;
                    ViewState["LogonAttempts"] = logonAttempts;
                    break;
                case Enums.eLogonResult.PasswordExpired:
                    GoToChangePassword(isPasswordExpired: true);
                    break;
                case Enums.eLogonResult.AccountDeleted:
                    lblMessage.Text = "This account has been deleted. Please contact your local/site administrator.";
                    lblMessage.Visible = true;
                    break;
                case Enums.eLogonResult.SystemError:
                    lblMessage.Text = "An error has occurred. Please try again later or contact us at the helpdesk for futher information.";
                    lblMessage.Visible = true;
                    break;
            }

        }

        #region Private Instance Methods

        private void ProcessLogon()
        {
            this.ProcessLogon(txtUserName.Text, rememberMe: chkRememberMe.Checked, redirectOnceLoggedIn: true);
        }

        private void ProcessLogon(string username, bool rememberMe, bool redirectOnceLoggedIn)
        {
            var outcome = Security.Authentication.ProcessAuthenticatedUser(username, rememberMe, redirectOnceLoggedIn, Request.QueryString["k"]);

            switch (outcome)
            {
                case Security.Authentication.eProcessUserOutcome.Success:
                    if (redirectOnceLoggedIn)
                        Response.Redirect(FormsAuthentication.GetRedirectUrl(username, rememberMe));
                    break;
                case Security.Authentication.eProcessUserOutcome.AccessDenied:
                    Response.Redirect("accessdenied.aspx");
                    break;
                case Security.Authentication.eProcessUserOutcome.RedirectToCustomerUserLandingPage:
                    Response.Redirect(Globals.Configuration.CustomeUserLandingPage);
                    break;
                case Security.Authentication.eProcessUserOutcome.RedirectToClientPortal:
                    this.RenderClientPortalRedirectScript();
                    break;
            }
        }

        private void LogOff(bool displayMessage, bool redirectToHasLoggedOff = true)
        {
            if (displayMessage)
            {
                lblMessage.Text = "You have logged off.";
                lblMessage.Visible = true;
            }

            Security.Authentication.LogOff();

            if (redirectToHasLoggedOff)
                Response.Redirect(Request.Url.AbsolutePath + "?HasLoggedOff=1");
        }

        private void GoToChangePassword(bool isPasswordExpired = false)
        {
            var username = txtUserName.Text;
            this.ProcessLogon(username, rememberMe: false, redirectOnceLoggedIn: false);

            var url = string.Format("~/security/changepassword.aspx?username={0}&ReturnUrl={1}", username, Request.QueryString["ReturnUrl"]);

            if (isPasswordExpired)
                url += "&Expired=true";

            // Would usually use Server.Transfer but require the Querystring variables to be set.
            Response.Redirect(url);
        }

        private void RenderClientPortalRedirectScript()
        {
            var script = string.Format(
                "var result = confirm('The Proteo Enterprise client portal has moved to {0}.\\n\\nPlease click OK to be redirected to the new client portal.');\nif (result) {{ window.location.replace('{0}'); }} else {{ window.location.replace('/security/login.aspx?LO=1'); }}",
                Orchestrator.Globals.Configuration.ClientPortalURL);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect_client", script, true);
        }

        private void RevokeDragAndDropPlanningAuthCookie()
        {
            HttpCookie userDataCookie = new HttpCookie("userData");
            userDataCookie.Path = "/";
            userDataCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(userDataCookie);
        }

        #endregion
    }
}
