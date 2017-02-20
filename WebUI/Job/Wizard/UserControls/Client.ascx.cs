namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;


	/// <summary>
	///	Allows the selection of the client 
	/// </summary>
	public partial class SelectClient : System.Web.UI.UserControl
	{
		#region Form Elements


		//protected CheckBox					chkCreateClient;
		//protected Orchestrator.WebUI.UserControls.BusinessRuleInfringementDisplay	infringementDisplay;


		#endregion

		#region Page Variables

		private Entities.Job	m_job;
		private int				m_jobId;
		private bool			m_isStock = false;
		private bool			m_isUpdate = false;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (Page.IsCallback) return;

			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			m_isStock = Convert.ToBoolean(Request.QueryString["isStock"]);

			if (m_jobId > 0)
			{
				m_isUpdate = true;
				cboClient.Enabled = false;
                cboBusinessType.Enabled = false;
			}

			if (m_isUpdate)
				GoToStep("JD");

			if (!IsPostBack)
			{
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                cboBusinessType.DataSource = facBusinessType.GetAll();
                cboBusinessType.DataBind();

                cboClient.Focus();
				if (Session[wizard.C_JOB] != null)
				{
					m_job = (Entities.Job) Session[wizard.C_JOB];
					PopulateClient();
                    cboBusinessType.ClearSelection();
                    cboBusinessType.Items.FindByValue(m_job.BusinessTypeID.ToString()).Selected = true;
				}
			}
			else
				m_job = (Entities.Job) Session[wizard.C_JOB];

//			infringementDisplay.Visible = false;
		}

		private void SelectClient_Init(object sender, EventArgs e)
		{
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

       void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text, false);

            int itemsPerRequest = 15;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step + "&isStock=" + m_isStock.ToString();
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void LoadJob()
		{
			// Load the job
			Facade.IJob facJob = new Facade.Job();
			m_job = facJob.GetJob(m_jobId);

			// Store the job in the session
			Session[wizard.C_JOB] = m_job;
		}

		private void PopulateClient()
		{
			// Get the organisation
			Facade.IOrganisation facOrganisation = new Facade.Organisation();

			// Configure the DbCombo box
			cboClient.Text = facOrganisation.GetForIdentityId(m_job.IdentityId).OrganisationName;
			cboClient.SelectedValue = m_job.IdentityId.ToString();

			// Can't change the client for a job once it is created.
			if (m_isUpdate)
				cboClient.Enabled = false;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			int identityId  = -1;

			if (cboClient.SelectedValue == "")
			{
				// The user has not selected an organistion, they may want to create a new one.
				pnlCreateNewOrganisation.Visible = true;

//				if (chkCreateClient.Checked)
//				{
//					Facade.IOrganisation facOrganisation = new Facade.Organisation();
//
//					Entities.Organisation organisation = new Entities.Organisation();
//					organisation.OrganisationName = cboClient.Text;
//					organisation.OrganisationType = eOrganisationType.Client;
//					organisation.IdentityStatus = eIdentityStatus.Active;
//
//					string userId = ((Entities.CustomPrincipal) Page.User).UserName;
//
//					Entities.FacadeResult retVal = facOrganisation.Create(organisation, userId);
//					if (!retVal.Success)
//					{
//						infringementDisplay.Infringements = retVal.Infringements;
//						infringementDisplay.DisplayInfringments();
//						infringementDisplay.Visible = true;
//					}
//					else
//						identityId = retVal.ObjectId;
//				}
			}
			else
				identityId = Convert.ToInt32(cboClient.SelectedValue);
			
			if (identityId > -1)
			{
				if (!m_isUpdate)
				{
					if (m_job != null)
					{
						if (m_job.IdentityId != identityId)
						{
							// The identity id has been changed, remove all the instructions and
							// references from the job.
							m_job.References = null;
							m_job.Instructions = null;
							
							// Update the identityId
							m_job.IdentityId = identityId;
						}

                        int businessTypeID = 0;
                        int.TryParse(cboBusinessType.SelectedValue, out businessTypeID);
                        m_job.BusinessTypeID = businessTypeID;

                        // Update the session variable
                        Session[wizard.C_JOB] = m_job;
                    }
					else
					{
						m_job = new Entities.Job();
						m_job.IdentityId = identityId;
						m_job.JobState = eJobState.Booked;
                        int businessTypeID = 0;
                        int.TryParse(cboBusinessType.SelectedValue, out businessTypeID);
                        m_job.BusinessTypeID = businessTypeID;

						// Update the session variable
						Session[wizard.C_JOB] = m_job;
					}
				}

				GoToStep("JT");
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
			this.Init += new EventHandler(SelectClient_Init);
		}
		#endregion
	}
}
