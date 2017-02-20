using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for tabPCVS.
	/// </summary>
	public partial class tabPCVS : Orchestrator.Base.BasePage
	{

		private const string C_JOB_VS = "C_JOB_VS";
		private const string C_POINT_ID_VS = "C_POINT_ID_VS";

		#region Fields
		
		#region Private 

		#endregion

		#region Protected
		protected	Entities.Job			m_job				= null;
		protected	int						m_jobId				= 0;	// The id of the job we are currently manipulating.
		protected	int						m_instructionId		= 0;	// The id of the job we are currently manipulating.
		protected	int						m_pointId			= 0;	// The id of the point visited in the job we are currently manipulating.
		#endregion

		#endregion		
		
		#region Page Load /Init

		protected void Page_Load(object sender, EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
			if (!IsPostBack)
			{
				// Bind the job information.
				BindJob();

				BindInstruction();
			}
			else
			{
				m_job = (Entities.Job) ViewState[C_JOB_VS];
				m_pointId = (int) ViewState[C_POINT_ID_VS];
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
			dgPCVs.ItemDataBound += new DataGridItemEventHandler(dgPCVs_ItemDataBound);

		}
		#endregion

		#endregion

		#region Data Loading/Binding
	    /// <summary>
		/// Retrieved the pcvs that apply to this point and populates the pcvs datagrid.
		/// </summary>
		private void BindPCVs()
		{
			using (Facade.IPCV facPCV = new Facade.PCV())
			{
				DataSet dsPCVs = facPCV.GetForJobIdAndDeliveryPointId(m_jobId, m_pointId);
				dgPCVs.DataSource = dsPCVs;
				dgPCVs.DataBind();
			}
		}

		private void BindInstruction()
		{
			Entities.Instruction instruction = null;

			// Get the instruction to work with.
			using (Facade.IInstruction facInstruction = new Facade.Instruction())
			{
				if (m_instructionId == 0)
				{
					// Get the job's next instruction.
					instruction = facInstruction.GetNextInstruction(m_jobId);
				}
				else
				{
					// Get the specific instruction.
					instruction = facInstruction.GetInstruction(m_instructionId);
				}
			}

			if (instruction != null)
			{
				m_pointId = instruction.PointID;
				ViewState[C_POINT_ID_VS]  = m_pointId;
				BindPCVs();
			}
		}

		private void BindJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				LoadJob();
			}
		}

        private void LoadJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				m_job = facJob.GetJob(m_jobId, true);

				if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);
			}

			ViewState[C_JOB_VS] = m_job;
		}

		private void dgPCVs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				int jobOfIssue = (int) ((DataRowView) e.Item.DataItem)["JobOfIssue"];

				e.Item.Attributes.Add("onClick", "javascript:HighlightRow('" + e.Item.ClientID + "');");
				e.Item.Attributes.Add("id", e.Item.ClientID);

                if (jobOfIssue == m_jobId)
                {
                    e.Item.Cells[0].Text = String.Empty;
                    e.Item.Cells[7].Text = "Issued";
                }
                else
                    e.Item.Cells[7].Text = "Attached";
			}
		}
		#endregion
	}
}
