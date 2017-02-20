using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Orchestrator.WebUI
{
    public partial class invalidoperation : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindError();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnSend.Click += new EventHandler(btnSend_Click);
        }

        void btnSend_Click(object sender, EventArgs e)
        {
            string moreInformation = Server.UrlEncode(txtMoreInformation.Text);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                Orchestrator.Globals.Configuration.MailFromName);

            mailMessage.To.Add("t.lunken@p1tp.com");
            mailMessage.To.Add("t.lunken@p1tp.com");
            mailMessage.To.Add("support@p1tp.com");
            mailMessage.To.Add("support@orchestrator.co.uk");

            mailMessage.Subject = "Orchestrator Error";

            StringBuilder sb = new StringBuilder();

            sb.Append("GUID: " + this.ViewState["_guid"].ToString());
            sb.Append(Environment.NewLine);

            sb.Append(moreInformation);
            if (Page.User != null && Page.User.GetType() == typeof(Orchestrator.Entities.CustomPrincipal))
                sb.Append("User:" + ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);
            else
                sb.Append("User:" + this.Page.User.Identity.Name);

            mailMessage.IsBodyHtml = true;
            mailMessage.Body = sb.ToString();

            SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = Globals.Configuration.MailServer;
            smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                Globals.Configuration.MailPassword);

            smtp.Send(mailMessage);
            mailMessage.Dispose();

            this.txtMoreInformation.Visible = false;
            this.lblThankYou.Visible = true;

        }
        private void BindError()
        {
            string errorGuid = Guid.NewGuid().ToString();
            this.ViewState["_guid"] = errorGuid;

            string errorMessage = String.Empty;

            Exception ex = Orchestrator.WebUI.Utilities.LastError;
            if (Orchestrator.WebUI.Utilities.LastError != null)
                if (ex == null)
                    if (Orchestrator.WebUI.Utilities.LastError.GetBaseException() != null)
                        ex = Orchestrator.WebUI.Utilities.LastError.GetBaseException();
            if (ex.InnerException != null)
            {
                errorMessage = FormatError(ex.InnerException);
                errorDetails.Controls.Add(new LiteralControl(errorMessage));
                SendEmail(ex);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["aspxerrorpath"]))
                hlReturn.NavigateUrl = Request.QueryString["aspxerrorpath"];
            else
            {
                if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_REFERER"]))
                    hlReturn.NavigateUrl = Request.ServerVariables["HTTP_REFERER"];
                else
                {
                    hlReturn.NavigateUrl = "~/default.aspx";
                    hlReturn.Text = "Click here to return to the homepage.";

                }
            }
        }
        private string FormatError(Exception ex)
        {
            string errorHead = "<table>";
            string errorFoot = "</table>";
            string localError = String.Empty;

            localError += errorHead;
            localError += "<tr><td valign=\"top\" width=\"100\"><b>Message</b></td><td>" + ex.Message + "</td></tr>";
            localError += "<tr><td valign=\"top\" width=\"100\"><b>Source</b></td><td>" + ex.Source + "</td></tr>";
            localError += "<tr><td valign=\"top\" width=\"100\"><b>Stack Trace</b></td><td>" + ex.StackTrace + "</td></tr>";
            localError += errorFoot + "<br/>";

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                localError += FormatError(ex);
            }

            return localError;
        }

        private void SendEmail(Exception ex)
        {
            MailMessage mailMessage = new System.Net.Mail.MailMessage();

            mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress, 
                Orchestrator.Globals.Configuration.MailFromName);

            mailMessage.To.Add(new MailAddress("t.lunken@p1tp.com"));
            mailMessage.To.Add(new MailAddress("t.lunken@p1tp.com"));
            mailMessage.To.Add(new MailAddress("support@p1tp.com"));
            mailMessage.To.Add(new MailAddress("support@orchestrator.co.uk"));

            mailMessage.Subject = "Orchestrator Error";

            StringBuilder sb = new StringBuilder();

            sb.Append("Event Type:\tError");
            sb.Append(Environment.NewLine);
            sb.Append("Event Source:\t" + ex.Source);
            sb.Append(Environment.NewLine);
            sb.Append("Date:\t" + DateTime.Now.ToShortDateString());
            sb.Append(Environment.NewLine);
            sb.Append("Time:\t" + DateTime.Now.ToShortTimeString());
            sb.Append(Environment.NewLine);
            if (Page.User != null && Page.User.GetType() == typeof(Orchestrator.Entities.CustomPrincipal))
                sb.Append("User:\t" + ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);
            else
                sb.Append("User:\tN/A");
            sb.Append(Environment.NewLine);
            sb.Append("Computer:\t" + Environment.MachineName);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Event message:\t" + ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append("Event time:\t" + DateTime.Now.ToString());
            sb.Append(Environment.NewLine);
            sb.Append("Event time (UTC):\t" + DateTime.UtcNow.ToString());
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            Exception iex = ex;
            while (iex != null)
            {
                sb.Append("Exception information:");
                sb.Append(Environment.NewLine);
                sb.Append("\tException type:\t" + iex.GetType().ToString());
                sb.Append(Environment.NewLine);
                sb.Append("\tException message:\t" + iex.Source);
                sb.Append(Environment.NewLine);
                sb.Append("Request information:");
                sb.Append(Environment.NewLine);
                sb.Append("\tRequest URL: " + Request.ServerVariables["HTTP_REFERER"]);
                sb.Append(Environment.NewLine);
                sb.Append("\tRequest path: " + Request.ServerVariables["HTTP_REFERER"]);
                sb.Append(Environment.NewLine);
                sb.Append("\tUser host address: " + Request.ServerVariables["REMOTE_HOST"]);
                sb.Append(Environment.NewLine);
                if (Page.User != null && Page.User.GetType() == typeof(Orchestrator.Entities.CustomPrincipal))
                    sb.Append("\tUser:" + ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);
                else
                    sb.Append("\tUser:");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                sb.Append("Thread information:");
                sb.Append(Environment.NewLine);
                sb.Append("\tStack trace: " + iex.StackTrace);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                iex = iex.InnerException;
            }
            sb.Append(Environment.NewLine);
            sb.Append("GUID: " + this.ViewState["_guid"].ToString());

            mailMessage.IsBodyHtml = false;
            mailMessage.Body = sb.ToString();

            SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = Globals.Configuration.MailServer;
            smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                Globals.Configuration.MailPassword);

            smtp.Send(mailMessage);
            mailMessage.Dispose();
            
        }
    }
}