namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for Returns.
	/// </summary>
    public partial class Returns : System.Web.UI.UserControl, IDefaultButton
    {

        #region IDefaultButton
        public System.Web.UI.Control DefaultButton
        {
            get { return this.btnNext; }
        }
        #endregion
		#region Form Elements



		#endregion

		#region Page Variables

		private Entities.Job	m_job;
		private int				m_jobId;
		private bool			m_isUpdate = false;

		private Entities.Instruction	m_instruction;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			m_job = (Entities.Job) Session[wizard.C_JOB];
			m_instruction = (Entities.Instruction) Session[wizard.C_INSTRUCTION];

			if (!IsPostBack)
			{
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

				switch ((eInstructionType) m_instruction.InstructionTypeId)
				{
					case eInstructionType.Load:
						lblCollection.Visible = true;
						lblDelivery.Visible = false;
						break;
					case eInstructionType.Drop:
						lblCollection.Visible = false;
						lblDelivery.Visible = true;
						break;
				}
			}		
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			btnBack.Click += new EventHandler(btnBack_Click);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			int m_collectDropIndex = m_instruction.CollectDrops.Count - 1;
			Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
			Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];

			GoToStep("PD");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				GoToStep("PP");
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(Page_Init);
		}
		#endregion
	}
}
