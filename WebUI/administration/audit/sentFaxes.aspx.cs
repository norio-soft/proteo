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

using Orchestrator.Globals;

namespace Orchestrator.WebUI.administration.audit
{
	/// <summary>
	/// Summary description for sentFaxes.
	/// </summary>
	public partial class sentFaxes : Orchestrator.Base.BasePage
	{
		#region Property Interfaces

		private string FaxSortCriteria
		{
			get { return (string) ViewState["FaxSortCriteria"]; }
			set { ViewState["FaxSortCriteria"] = value; }
		}

		private string FaxSortDirection
		{
			get { return (string) ViewState["FaxSortDirection"]; }
			set { ViewState["FaxSortDirection"] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.SystemUsage);

			if (Request.QueryString["GETFAXIMAGE"] == "true")
			{
				//Header1.Title = "Sent Fax";
				//Header1.SubTitle = "There was a problem retrieving the sent fax.";
				ReturnImageData(Convert.ToInt64(Request.QueryString["TransactionId"]));
				mainTable.Visible = false;
				lblConfirmation.Visible = true;
			}
			else
			{
				if (!IsPostBack)
				{
					// Configure the filter date range (last week)
					DateTime now = DateTime.UtcNow;
					now = now.Subtract(now.TimeOfDay);
                    dteStartDate.SelectedDate = now.Subtract(new TimeSpan(6, 0, 0, 0));
                    dteEndDate.SelectedDate = now.Add(new TimeSpan(23, 59, 59));

					// Configure the default sort criteria]
					FaxSortCriteria = "CreateDate";
					FaxSortDirection = "DESC";

					BindFaxData();
				}

				lblConfirmation.Visible = false;
			}
		}

		protected void sentFaxes_Init(object sender, EventArgs e)
		{
			btnFilter.Click += new EventHandler(btnFilter_Click);
		}

		#endregion

		#region Methods & Events
		private void BindFaxData()
		{
			Facade.IAudit facAudit = new Facade.Audit();
            DataSet ds = facAudit.GetFaxes(dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);

			DataView dv = new DataView(ds.Tables[0]);
			string sort = FaxSortCriteria + " " + FaxSortDirection;
			dv.Sort = sort.Trim();

			dgFaxes.DataSource = dv;
			dgFaxes.DataBind();

			if (dgFaxes.Items.Count != 0)
			{
				pnlFaxes.Visible = true;
				dgFaxes.Visible = true;
				lblNote.Visible = false;				
			}
			else
			{
				pnlFaxes.Visible = false;
				dgFaxes.Visible = false;
				lblNote.Visible = true;
				lblNote.Text = "No sent faxes for given criteria.";
				lblNote.ForeColor = Color.Red;
			}
		}

		private void ReturnImageData(long transactionId)
		{
			byte[] bytes = new byte[1];
			InterFax.InterFax myInterFax = new InterFax.InterFax();

			try
			{
				long returnValue = myInterFax.GetFaxImage(Configuration.InterFaxUserName, Configuration.InterFaxPassword, transactionId, ref bytes);

				if (returnValue == 0)
				{
					Response.Clear();
					Response.ContentType = "image/tiff";
					Response.AddHeader("Content-Disposition", "attachment; filename=" + transactionId.ToString() + ".tiff");
					Response.OutputStream.Write(bytes, 0, bytes.Length);
					Response.End();
				}
				else
				{
					lblConfirmation.Text = "The requested fax could not be retrieved, the error code returned was: " + returnValue.ToString();
					Orchestrator.Logging.ApplicationLog.WriteError("Orchestrator.WebUI.adminstration.audit.sentFaxes.ReturnImageData", "Fax retrieval failed: " + returnValue.ToString());
				}
			}
			catch {}
		}

		#endregion
		
		#region Event Handlers

		#region Button

		private void btnFilter_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				dgFaxes.CurrentPageIndex = 0;
				BindFaxData();
			}
		}

        public void btnResend_Click(object sender, EventArgs e)
        {
            int count = int.MinValue; 
            long transactionId = long.MinValue;
            string faxNumber = string.Empty;
            eReportType reportType = 0;
            InterFax.InterFax myInterFax;

            count = dgFaxes.SelectedItems.Count;
            if (count != 0)
            {
                // Should only have one select as this is multiple selects on the grid are not allowed.
                foreach (ComponentArt.Web.UI.GridItem item in dgFaxes.SelectedItems)
                {
                    transactionId = Convert.ToInt64(item["TransactionId"]);
                    faxNumber = item["FaxNumber"].ToString ();
                    reportType = (eReportType)Enum.Parse(typeof(eReportType), item["Description"].ToString());
                }

                myInterFax = new InterFax.InterFax();
                long returnedTransactionId = myInterFax.ReSendFax(Configuration.InterFaxUserName, Configuration.InterFaxPassword, transactionId, faxNumber);

                if (returnedTransactionId > 0)
                {
                    Facade.IAudit facAudit = new Facade.Audit();
                    facAudit.FaxSent(reportType, returnedTransactionId, faxNumber, ((Entities.CustomPrincipal)Page.User).UserName);

                    lblConfirmation.Text = "The fax was resent.";

                    // Cause the data to be updated.
                    BindFaxData();
                }
                else
                {
                    lblConfirmation.Text = "The fax was not resent.";
                    Orchestrator.Logging.ApplicationLog.WriteError("Orchestrator.WebUI.adminstration.audit.sentFaxes.ResendFax", "Fax resend failed: " + returnedTransactionId);
                }

                lblConfirmation.Visible = true;
            }
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
			this.Init += new System.EventHandler(this.sentFaxes_Init);

		}
		#endregion
	}
}
