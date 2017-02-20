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

namespace Orchestrator.WebUI.Traffic
{
	/// <summary>
	/// Functions as a portal to the job management activities.
	/// </summary>
	public partial class jobManagement : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private int m_jobId = 0;
		private Entities.Job m_job;

		#endregion

		#region Page Load / Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (LoadJob())
			{
				// Jump to the appropriate page
				Portal();
			}
			else
			{
				lblMessage.Visible = true;
				lblMessage.Text += Request.QueryString["jobId"] + ".";
			}
		}

		private void jobManagement_Init(object sender, EventArgs e)
		{
			btnReturnToTrafficSheet.Click += new EventHandler(btnReturnToTrafficSheet_Click);
		}

		#endregion

		#region Event Handlers

		private void btnReturnToTrafficSheet_Click(object sender, EventArgs e)
		{
			Response.Redirect("trafficSheet.aspx");
		}

		#endregion

		private bool LoadJob()
		{
			bool isJobLoaded = false;

			try
			{
				// Retrieve the Job Id
				m_jobId = Int32.Parse(Request.QueryString["jobId"]);

				// Load the Job
				Facade.IJob facJob = new Facade.Job();
				m_job = facJob.GetJob(m_jobId);

				// Check that the job has been loaded
				if (m_job != null)
					isJobLoaded = true;
			}
			catch {}

			return isJobLoaded;
		}
		
		private void Portal()
		{
			switch (m_job.JobState)
			{
				case eJobState.Planned:
					// Job has been fully assigned, but the driver has not yet accepted the job
                    Response.Redirect("JobManagement/driverCommunications.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					break;
				case eJobState.InProgress:
					// Driver has accepted the job
                    Response.Redirect("JobManagement/DriverCallIn/Callin.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					break;
				case eJobState.Completed:
					// Driver has reported that the job is complete
                    Response.Redirect("JobManagement/bookingInPODs.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					break;
				case eJobState.BookingInIncomplete:
					// Booking In is incomplete
                    Response.Redirect("JobManagement/bookingInPODs.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					break;
				case eJobState.BookingInComplete:
					if (m_job.IsPriced)
					{
						// Display the job details
                        Response.Redirect("../Job/job.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					}
					else
					{
						// Booking In is complete but the job has not been priced
                        Response.Redirect("JobManagement/pricing2.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
					}
					break;
				default:
					// Display the job details
					Response.Redirect("../Job/job.aspx?wiz=true&jobId=" + m_jobId.ToString());
					break;
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
			this.Init += new EventHandler(jobManagement_Init);

		}
		#endregion
	}
}
