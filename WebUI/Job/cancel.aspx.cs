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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for cancel.
	/// </summary>
	public partial class cancel : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_JOB_VS	= "C_JOB_VS";

		#endregion

		#region Form Elements


		#endregion

		#region Page Variables

		private		int					m_jobId = 0;
		private		Entities.Job		m_job = null;

        public int JobTypeID { get; set; }
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);

			m_jobId = Convert.ToInt32(Request.QueryString["JobId"]);

			if (!IsPostBack)
			{
				if (m_jobId > 0)
				{
					LoadJob();
					PopulatePage();
				}
			}
			else
			{
				m_job = (Entities.Job) ViewState[C_JOB_VS];
			}

			lblConfirmation.Visible = false;
		}

		private void cancel_Init(object sender, EventArgs e)
		{
			btnCancelJob.Click += new EventHandler(btnCancelJob_Click);
		}

		private void LoadJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				m_job = facJob.GetJob(m_jobId);
                this.JobTypeID = (int)m_job.JobType;
				ViewState[C_JOB_VS] = m_job;
			}
		}

		private void PopulatePage()
		{
			// Populate the Job fieldset.
			lblJobId.Text = m_jobId.ToString();
			lblJobState.Text = Utilities.UnCamelCase(m_job.JobState.ToString());
			lblJobType.Text = Utilities.UnCamelCase(m_job.JobType.ToString());
			if (m_job.CurrentTrafficArea == null)
				lblCurrentTrafficArea.Text = "Unknown";
			else
				lblCurrentTrafficArea.Text = m_job.CurrentTrafficArea.TrafficAreaName;
			lblStockMovement.Text = (m_job.IsStockMovement ? "Yes" : "No");
			using (Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest())
			{
				if ((facPlannerRequest.GetPlannerRequestsForJobId(m_job.JobId)).Tables[0].Rows.Count > 0)
				{
					imgHasRequests.Visible = true;
					imgHasRequests.Attributes.Add("onClick", "javascript:ShowPlannerRequests('" + m_job.JobId.ToString() + "');");
				}
				else
					imgHasRequests.Visible = false;
			}

			// Populate the cancelation controls
			txtCancellationReason.Text = m_job.ForCancellationReason;

			if (m_job.JobState == eJobState.Cancelled || m_job.HasBeenPosted)
				btnCancelJob.Enabled = false;

            // No reason for cancellation is required if the job type is groupage as groupage jobs get deleted upon cancellation.
            cancellationTable.Visible = m_job.JobType != eJobType.Groupage;
		}

		#region Button Events

		private void btnCancelJob_Click(object sender, EventArgs e)
		{
            if (this.btnCancelJob.Text == "Close")
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseWindow", "window.close();", true);
            else
                if (Page.IsValid)
                {
                    bool result = false;
                    using (Facade.IJob facJob = new Facade.Job())
                    {
                        Facade.IInstruction facInstruction = new Facade.Instruction();
                        Entities.InstructionCollection insCol = facInstruction.GetForJobId(m_jobId);

                        if (insCol.Count == 0 || insCol[0].InstructionActuals == null || insCol[0].InstructionActuals.Count == 0)
                        {
                            result =
                                facJob.UpdateState(m_jobId, eJobState.Cancelled,
                                                   ((Entities.CustomPrincipal)Page.User).UserName);

                            if (result)
                            {
                                lblConfirmation.Text = "The Job " + m_jobId + " has been cancelled successfully.";
                                lblConfirmation.Visible = true;
                                //LoadJob();
                                //PopulatePage();

                                this.ButtonBar.Visible = false;
                                this.cancellationTable.Visible = false;
                                this.jobDetailsFieldset.Visible = false;
                                this.btnCancelJob.Text = "Close";
                                this.jobDetailsTD.Width = "0%";
                                this.cancellationReasnTD.Width = "100%";
                            }
                            else
                            {
                                lblConfirmation.Text = "The job could not be cancelled.";
                                lblConfirmation.Visible = true;
                            }
                        }
                        else
                        {
                            lblConfirmation.Visible = true;
                            lblConfirmation.Text = "This job can not be cancelled at this time as it has at least one call-in.";
                        }
                    }
                }
		}

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
			this.Init += new EventHandler(cancel_Init);
		}
		#endregion
	}
}
