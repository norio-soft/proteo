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

using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

using Orchestrator.Logging;

namespace Orchestrator.WebUI.scan
{
    public partial class emailPDF : Orchestrator.Base.BasePage
    {
        private string _pdfLocation = string.Empty;
        private eFormTypeId _formTypeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _formTypeId = (eFormTypeId)Enum.Parse(typeof(eFormTypeId), Request.QueryString["type"]);
            _pdfLocation = Request.QueryString["pdfLocation"];
            pnlConfirmation.Visible = false;

            if (!IsPostBack)
                revEmailAddress.ValidationExpression = Orchestrator.Globals.Constants.EMAIL_REGULAR_EXPRESSION.ToString();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool success = SendPDF();

                if (success)
                {
                    mwhelper.CloseForm = true;
                    mwhelper.CausePostBack = false;
                }
                else
                {
                    pnlConfirmation.Visible = true;
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CloseForm = true;
            mwhelper.CausePostBack = false;
        }

        private bool SendPDF()
        {
            bool success = false;

            try
            {
                // Retrieve the PDF to send via email.
                string url = _pdfLocation;
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                ((HttpWebRequest)request).UserAgent = "Orchestrator";
                WebResponse response = request.GetResponse();

                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    // PDF has been received so we need to create the email and send it on it's way.
                    // Send the pdf document to the email address supplied.
                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                        Orchestrator.Globals.Configuration.MailFromName);

                    mailMessage.To.Add(txtEmailAddress.Text);
                    mailMessage.Subject = "Requested " + _formTypeId.ToString();
                    mailMessage.IsBodyHtml = false;
                    mailMessage.Body = GenerateBodyText(url);

                    // Close the request object.
                    response.Close();

                    try
                    {
                        SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = Globals.Configuration.MailServer;
                        smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                            Globals.Configuration.MailPassword);

                        smtp.Send(mailMessage);
                        success = true;
                        mailMessage.Dispose();

                    }
                    catch (Exception eX)
                    {
                        ApplicationLog.WriteError("SendPDF", eX.Message);
                    }
                }
            }
            catch (Exception eX)
            {
                ApplicationLog.WriteError("SendPDF", eX.Message);
            }

            return success;
        }

        private string GenerateBodyText(string url)
        {
            return "The " + _formTypeId.ToString() + " you requested is available at " + url + ".\n\nThe file is in PDF format, if you have any problems opening the file you may need to install Acrobat Reader which you can download free from (http://www.adobe.com/products/acrobat/readstep2.html).";
        }
    }
}
