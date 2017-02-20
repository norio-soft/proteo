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
using System.Xml;
using System.Xml.Xsl;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Allows users to cancel a job.
	/// </summary>
	public partial class cancel : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private int				m_jobId;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditJob);

			m_jobId = Convert.ToInt32(Request.QueryString["JobId"]);
			lblConfirmation.Visible = false;

			if (!IsPostBack)
				LoadJob();
		}

		private void cancel_Init(object sender, EventArgs e)
		{
			btnCancelJob.Click += new EventHandler(btnCancelJob_Click);
		}

		#endregion

		private void LoadJob()
		{
			Facade.IJob facJob = new Facade.Job();
			Entities.Job job = facJob.GetJob(m_jobId);

			bool canBeMarkedForCancellation = job.JobState == eJobState.Booked || job.JobState == eJobState.Planned || job.JobState == eJobState.InProgress;

			// Set the checkbox status and the cancellation reason.
			chkMarkforCancellation.Checked = job.ForCancellation;
			if (job.ForCancellation)
				txtCancellationReason.Text = job.ForCancellationReason;

			if (!canBeMarkedForCancellation || job.ForCancellation)
			{
				// Only allow the display of the cancellation information.
				chkMarkforCancellation.Enabled = false;
				txtCancellationReason.Enabled = false;

				// This job is too advanced to be cancelled, or has already been marked for cancellation.
				btnCancelJob.Enabled = false;
			}
		}

		private void btnCancelJob_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				string userId = ((Entities.CustomPrincipal) Page.User).UserName;

				Facade.IJob facJob = new Facade.Job();
				bool success = facJob.UpdateForCancellation(m_jobId, chkMarkforCancellation.Checked, txtCancellationReason.Text, userId);

				if (success)
					lblConfirmation.Text = "The job was marked as cancelled.";
				else
					lblConfirmation.Text = "The job was not marked as cancelled, please try again.";
				
				lblConfirmation.Visible = true;
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
			this.Init += new EventHandler(cancel_Init);
		}
		#endregion
	}
}