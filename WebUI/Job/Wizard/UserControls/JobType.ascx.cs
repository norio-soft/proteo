namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for JobType.
	/// </summary>
	public partial class JobType : System.Web.UI.UserControl, IDefaultButton    
	{

        public System.Web.UI.Control DefaultButton
        {
            get {return this.btnNext;}
        }

		#region Form Elements



		#endregion

		#region Page Variables

		private Entities.Job	m_job;
		private int				m_jobId;
		private bool			m_isStock = false;
		private bool			m_isUpdate = false;
		private bool			m_isAmendment = false;

		#endregion

        
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			m_isStock = Convert.ToBoolean(Request.QueryString["isStock"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			m_job = (Entities.Job) Session[wizard.C_JOB];
			m_isAmendment = m_job.Charge != null;

			if (!IsPostBack)
			{
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);
				txtChargeAmount.Attributes.Add("onkeydown", "checkKeyPress();");

                // Set the Default Button for the form
               


				PopulateStaticControls();
				// use defaults unless this is an existing job, or an amendment to a pending job
				bool useDefaults = !(m_isUpdate || m_isAmendment);
				PopulateType(useDefaults);		
				
				if (!m_isAmendment)
				{
					if (m_isStock)
						chkIsStockMovement.Checked = true;
					else
						chkIsStockMovement.Checked = false;
				}
			}
		}

		private void JobType_Init(object sender, EventArgs e)
		{
			cboChargeType.SelectedIndexChanged += new EventHandler(cboChargeType_SelectedIndexChanged);

			btnBack.Click += new EventHandler(btnBack_Click);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}
		
		private void ConfigureChargeAmount(eJobChargeType chargeType)
		{
			bool enable = (chargeType == eJobChargeType.Job || chargeType == eJobChargeType.PerPallet) && (m_job.JobState != eJobState.Cancelled || m_job.JobState != eJobState.Invoiced);

			txtChargeAmount.Enabled = enable;
			rfvChargeAmount.Enabled = enable;
			revChargeAmount.Enabled = enable;
		}

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void PopulateStaticControls()
		{
			cboChargeType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobChargeType)));
			cboChargeType.DataBind();
		}

		private void PopulateType(bool useClientDefaults)
		{
			eJobType jobType = eJobType.Normal;
			eJobChargeType jobChargeType = eJobChargeType.Job;
			decimal chargeAmount = 0;

			if (useClientDefaults)
			{
				Facade.IOrganisation facOrganisation = new Facade.Organisation();
				Entities.Organisation client = facOrganisation.GetForIdentityId(m_job.IdentityId);

				if (client.Defaults != null && client.Defaults.Count > 0)
					jobType = (eJobType) client.Defaults[0].JobTypeId;
			}
			else
			{
				// Get the values from the Job object
				jobType = m_job.JobType;
				jobChargeType = m_job.Charge.JobChargeType;
				chargeAmount = m_job.Charge.JobChargeAmount;
			}

			cboChargeType.ClearSelection();
			cboChargeType.Items.FindByValue(Utilities.UnCamelCase(Enum.GetName(typeof(eJobChargeType), jobChargeType))).Selected = true;

			ConfigureChargeAmount(jobChargeType);

			txtChargeAmount.Text = chargeAmount.ToString("F2");

			chkIsStockMovement.Checked = m_job.IsStockMovement;
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			bool jumpToDetails = Session[wizard.C_JUMP_TO_DETAILS] != null && ((bool) Session[wizard.C_JUMP_TO_DETAILS]);
			Session[wizard.C_JUMP_TO_DETAILS] = false;

			if (jumpToDetails)
				GoToStep("JD");
			else
				GoToStep("SC");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				m_job.JobType = eJobType.Normal;
				
				if (m_job.Charge == null)
					m_job.Charge = new Entities.JobCharge();

				m_job.Charge.JobChargeType = (eJobChargeType) Enum.Parse(typeof(eJobChargeType), cboChargeType.SelectedValue.Replace(" ", ""));

				if (m_job.Charge.JobChargeType == eJobChargeType.Job || m_job.Charge.JobChargeType == eJobChargeType.PerPallet)
                    m_job.Charge.JobChargeAmount = Decimal.Parse(txtChargeAmount.Text, System.Globalization.NumberStyles.Currency);
				else
					m_job.Charge.JobChargeAmount = 0;
				m_job.IsStockMovement = chkIsStockMovement.Checked;

				Session[wizard.C_JOB] = m_job;

				if (m_isUpdate)
				{
					string userId = ((Entities.CustomPrincipal) Page.User).UserName;

					Facade.IJob facJob = new Facade.Job();
					facJob.Update(m_job, userId);

					Session[wizard.C_JOB] = null;
					GoToStep("JD");
				}
				else
				{
					bool jumpToDetails = Session[wizard.C_JUMP_TO_DETAILS] != null && ((bool) Session[wizard.C_JUMP_TO_DETAILS]);
					Session[wizard.C_JUMP_TO_DETAILS] = false;

					if (jumpToDetails)
						GoToStep("JD");
					else
						GoToStep("JR");
				}
			}
		}

		private void cboChargeType_SelectedIndexChanged(object sender, EventArgs e)
		{
			eJobChargeType selectedChargeType = (eJobChargeType) Enum.Parse(typeof(eJobChargeType), cboChargeType.SelectedValue.Replace(" ", ""));

			ConfigureChargeAmount(selectedChargeType);
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
			this.Init += new EventHandler(JobType_Init);
		}
		#endregion
	}
}
