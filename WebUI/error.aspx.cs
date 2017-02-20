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

using System.Text;

namespace Orchestrator.WebUI
{
	/// <summary>
	/// Summary description for error.
	/// </summary>
	public partial class error : Orchestrator.Base.BasePage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!IsPostBack)
				{
					BindError();
				}
			}
			catch{}
		}

		#region Web Form Designer generated code

		protected Panel pnlErrorInf;
		protected Label lblErrorMessage;
		protected Label lblStackTrace;
		protected Label lblErrorSource;
		protected Label lblErrorBaseException;
		protected DataGrid dgLastError;
		protected LinkButton btnShowHide;

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

		private void BindError()
		{
			string errorMessage = String.Empty;

			Exception ex = Utilities.LastError.GetBaseException();
			
			errorMessage = FormatError(ex);
			errorDetails.Controls.Add(new LiteralControl(errorMessage));
			//SendEmail(ex);
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

			if (ex.InnerException != null)
				localError += FormatError(ex.InnerException);

			return localError;
		}

		private void BindServerVariables()
		{
			DataTable t = new DataTable();
			t.Columns.Add(new DataColumn("server variable"));
			t.Columns.Add(new DataColumn("value"));

			foreach(string var in Request.ServerVariables.Keys)
			{
				DataRow row = t.NewRow();
				row[0] = var;
				row[1] = Request.ServerVariables[var].ToString();
				t.Rows.Add(row);
			}

			dgServerVariables.DataSource = t;
		}

        //private void SendEmail(Exception ex)
        //{
        //    ASPEMAILLib.IMailSender aspMail;
        //    aspMail = new ASPEMAILLib.MailSender();

        //    aspMail.FromName = Orchestrator.Globals.Configuration.MailFromName;
        //    aspMail.From = Orchestrator.Globals.Configuration.MailFromAddress;

        //    aspMail.AddAddress("t.lunken@p1tp.com", "t.lunken@p1tp.com");
        //    aspMail.AddAddress("t.lunken@p1tp.com", "t.lunken@p1tp.com");
        //    aspMail.AddAddress("support@p1tp.com", "support@p1tp.com");

        //    aspMail.Subject = "Orchestrator Error";

        //    aspMail.Host = Orchestrator.Globals.Configuration.MailServer;

        //    aspMail.Username = Orchestrator.Globals.Configuration.MailUsername;
        //    aspMail.Password = Orchestrator.Globals.Configuration.MailPassword;

        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("Event Type:\tError");
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Event Source:\t" + ex.Source);
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Date:\t" + DateTime.Now.ToShortDateString());
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Time:\t" + DateTime.Now.ToShortTimeString());
        //    sb.Append(Environment.NewLine);
        //    if (Page.User != null && Page.User.GetType() == typeof(Entities.CustomPrincipal))
        //        sb.Append("User:\t" + ((Entities.CustomPrincipal)Page.User).UserName);
        //    else
        //        sb.Append("User:\tN/A");
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Computer:\t" + Environment.MachineName);
        //    sb.Append(Environment.NewLine);
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Event message:\t" + ex.Message);
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Event time:\t" + DateTime.Now.ToString());
        //    sb.Append(Environment.NewLine);
        //    sb.Append("Event time (UTC):\t" + DateTime.UtcNow.ToString());
        //    sb.Append(Environment.NewLine);
        //    sb.Append(Environment.NewLine);

        //    Exception iex = ex;
        //    while (iex != null)
        //    {
        //        sb.Append("Exception information:");
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tException type:\t" + iex.GetType().ToString());
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tException message:\t" + iex.Source);
        //        sb.Append(Environment.NewLine);
        //        sb.Append("Request information:");
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tRequest URL: " + Request.ServerVariables["HTTP_REFERER"]);
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tRequest path: " + Server.MapPath(Request.ServerVariables["HTTP_REFERER"]));
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tUser host address: " + Request.ServerVariables["REMOTE_HOST"]);
        //        sb.Append(Environment.NewLine);
        //        if (Page.User != null && Page.User.GetType() == typeof(Entities.CustomPrincipal))
        //            sb.Append("\tUser:" + ((Entities.CustomPrincipal)Page.User).UserName);
        //        else
        //            sb.Append("\tUser:");
        //        sb.Append(Environment.NewLine);
        //        sb.Append(Environment.NewLine);

        //        sb.Append("Thread information:");
        //        sb.Append(Environment.NewLine);
        //        sb.Append("\tStack trace: " + iex.StackTrace);
        //        sb.Append(Environment.NewLine);
        //        sb.Append(Environment.NewLine);

        //        iex = iex.InnerException;
        //    }

        //    aspMail.Body = sb.ToString();

        //    aspMail.IsHTML = 0;
        //    aspMail.Send("");
        //}

	}
}
