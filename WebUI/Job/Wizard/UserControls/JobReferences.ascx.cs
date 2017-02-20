namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using P1TP.Components.Web.Validation;

	/// <summary>
	///		Summary description for JobReferences.
	/// </summary>
	public partial class JobReferences : System.Web.UI.UserControl, IDefaultButton
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

		private Entities.OrganisationReferenceCollection	m_clientReferences	= null;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			m_job = (Entities.Job) Session[wizard.C_JOB];

			ConfigureClientReferences();

			if (!IsPostBack)
			{
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

				ConfigureLoadTerminology();

				PopulateReferences();
			}
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			cfvLoadNumber.ServerValidate += new ServerValidateEventHandler(cfvLoadNumber_ServerValidate);
			repReferences.ItemDataBound += new RepeaterItemEventHandler(repReferences_ItemDataBound);

			btnBack.Click += new EventHandler(btnBack_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}

		private void ConfigureClientReferences()
		{
			if (m_isUpdate)
			{
				// Populate the repeater control with the current reference fields
				Entities.OrganisationReferenceCollection clientReferences = new Entities.OrganisationReferenceCollection();
				foreach (Entities.JobReference reference in m_job.References)
				{
					clientReferences.Add(reference.OrganisationReference);
				}

				repReferences.DataSource = clientReferences;
				repReferences.DataBind();
			}
			else
			{
				Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
				m_clientReferences = facOrganisationReference.GetReferencesForOrganisationIdentityId(m_job.IdentityId, true);

				if (m_clientReferences != null)
				{
					// Populate the repeater control with the required reference fields
					repReferences.DataSource = m_clientReferences;
					repReferences.DataBind();
				}
			}
		}

		private void ConfigureLoadTerminology()
		{
			Facade.IReferenceData facReferenceData = new Facade.ReferenceData();

			string loadNumberText = facReferenceData.GetLoadNumberTextForIdentityId(m_job.IdentityId);
			if (loadNumberText != String.Empty)
			{
				lblLoadNumber.Text = loadNumberText;
				rfvLoadNumber.ErrorMessage = "Please enter the " + loadNumberText + ".";
				rfvLoadNumber.Text = rfvLoadNumber.Text.Replace("Load Number.", loadNumberText + ".");
			}
		}

		private string GenerateLoadNumber()
		{
			string loadNumber = "AG";
			return loadNumber;
		}

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void PopulateReferences()
		{
			txtLoadNumber.Text = m_job.LoadNumber;
		}

		private void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			Entities.OrganisationReferenceCollection organisationReferences = (Entities.OrganisationReferenceCollection) repReferences.DataSource;

			HtmlInputHidden hidOrganisationReferenceId = (HtmlInputHidden) e.Item.FindControl("hidOrganisationReferenceId");
			PlaceHolder plcHolder = (PlaceHolder) e.Item.FindControl("plcHolder");

			int organisationReferenceId = Convert.ToInt32(hidOrganisationReferenceId.Value);

			if (!IsPostBack && m_job.References != null)
			{
				// Make sure the value is in place
				TextBox txtReferenceValue = (TextBox) e.Item.FindControl("txtReferenceValue");
				Entities.JobReference currentValue = m_job.References.GetForReferenceId(organisationReferenceId);
				if (currentValue != null)
					txtReferenceValue.Text = currentValue.Value;
			}

			// Configure the validator controls
			CustomValidator validatorControl = null;

            Entities.OrganisationReference reference = organisationReferences.FindByReferenceId(organisationReferenceId);
			switch (reference.DataType)
			{
				case eOrganisationReferenceDataType.Decimal:
					validatorControl = new CustomValidator();
					validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateDecimal);
					break;
				case eOrganisationReferenceDataType.FreeText:
					// No additional validation required.
					break;
				case eOrganisationReferenceDataType.Integer:
					validatorControl = new CustomValidator();
					validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateInteger);
					break;
			}

			if (validatorControl != null)
			{
				// Configure the validator properties
				validatorControl.ControlToValidate = "txtReferenceValue";
				validatorControl.Display = ValidatorDisplay.Dynamic;
				validatorControl.ErrorMessage = "Please supply a " + reference.Description + ".";
				validatorControl.Text = "<img src=\"../../images/error.png\"  Title=\"Please supply a " + reference.Description + ".\" />";
				validatorControl.EnableClientScript = false;

				plcHolder.Controls.Add(validatorControl);
			}

			if (m_isUpdate)
			{
				// Populate the text box with the value already configured for the reference
				TextBox txtReferenceValue = (TextBox) e.Item.FindControl("txtReferenceValue");

				// Find the reference
				Entities.JobReference jobReference = m_job.References.GetForReferenceId(organisationReferenceId);
				if (jobReference != null)
				{
					txtReferenceValue.Text = jobReference.Value;
				}
			}
		}

		private void cfvLoadNumber_ServerValidate(object source, ServerValidateEventArgs args)
		{
            args.IsValid = true;
            // Commented out as load numbers can now be reused - Stephen Newman
            //Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            //args.IsValid = facReferenceData.IsIdentityIdLoadNumberUnique(m_job.IdentityId, m_job.JobId, args.Value);
		}

		private void validatorControl_ServerValidateDecimal(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
		}

		private void validatorControl_ServerValidateInteger(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			bool jumpToDetails = Session[wizard.C_JUMP_TO_DETAILS] != null && ((bool) Session[wizard.C_JUMP_TO_DETAILS]);
			Session[wizard.C_JUMP_TO_DETAILS] = false;

			if (jumpToDetails)
				GoToStep("JD");
			else
				GoToStep("JT");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		protected void btnNext_Click(object sender, EventArgs e)
		{
			btnNext.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Populate the references
				if (txtLoadNumber.Text.Trim().Length == 0)
					txtLoadNumber.Text = GenerateLoadNumber();

				m_job.LoadNumber = txtLoadNumber.Text;

				// Ensure there is a reference collection available
				if (m_job.References == null)
					m_job.References = new Entities.JobReferenceCollection();

				int referenceId;
				string referenceValue;

				foreach (RepeaterItem item in repReferences.Items)
				{
					referenceId = Convert.ToInt32(((HtmlInputHidden) item.FindControl("hidOrganisationReferenceId")).Value);
					referenceValue = ((TextBox) item.FindControl("txtReferenceValue")).Text;

					// Find the existing reference field and update it
					Entities.JobReference jobReference = m_job.References.GetForReferenceId(referenceId);

					if (jobReference == null)
					{
						// Create a new reference and add it to the reference collection
						jobReference = new Entities.JobReference();
						jobReference.JobId = m_jobId;
						jobReference.Value = referenceValue;
						jobReference.OrganisationReference = m_clientReferences.FindByReferenceId(referenceId);

						m_job.References.Add(jobReference);
					}
					else
						jobReference.Value = referenceValue;
				}

				// Update the job session variable
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

					Session[wizard.C_POINT_TYPE] = ePointType.Collect;
					Session[wizard.C_INSTRUCTION] = null;

					if (jumpToDetails)
						GoToStep("JD");
					else
						GoToStep("P");
				}
			}
			else
			{
				if (!rfvLoadNumber.IsValid)
				{
					// Cause the load number validator to be ignored.
                    btnNext.NoFormValList = rfvLoadNumber.ClientID;
				}
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
