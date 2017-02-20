namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

    /// <summary>
    ///		Summary description for ClientsCustomer.
    /// </summary>
    public partial class ClientsCustomer : System.Web.UI.UserControl, IDefaultButton
    {
        #region IDefaultButton
        public System.Web.UI.Control DefaultButton
        {
            get { return this.btnNext; }
        }
        #endregion

        #region Page Variables

        private Entities.Job m_job;
        private int m_jobId;
        private bool m_isUpdate = false;
        private bool m_isAmendment = false;

        private Entities.Instruction m_instruction;
        private int m_instructionIndex;

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (m_jobId > 0)
                m_isUpdate = true;

            // Retrieve the job from the session variable
            m_job = (Entities.Job)Session[wizard.C_JOB];

            if (Session[wizard.C_INSTRUCTION_INDEX] != null)
            {
                m_instructionIndex = (int)Session[wizard.C_INSTRUCTION_INDEX];

                if (!m_isUpdate && m_instructionIndex != m_job.Instructions.Count)
                    m_isAmendment = true;
            }

            if (Session[wizard.C_INSTRUCTION] != null)
            {
                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

                if (m_isUpdate && m_isAmendment)
                {
                    // Get the organisation
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();

                    // Configure the DbCombo box
                    cboClient.Text = facOrganisation.GetForIdentityId(m_instruction.ClientsCustomerIdentityID).OrganisationName;
                    cboClient.SelectedValue = m_instruction.ClientsCustomerIdentityID.ToString();

                    // Can't change the client's customer for a drop once it is created.
                    if (m_isUpdate)
                        cboClient.Enabled = false;
                }
            }
            else
            {
                // This must be a new drop
                m_instruction = new Entities.Instruction();
                m_instruction.JobId = m_jobId;
                m_instruction.InstructionTypeId = (int)eInstructionType.Drop;

                Session[wizard.C_INSTRUCTION] = m_instruction;

                // If this is a new job, set the instruction index
                if (!m_isUpdate)
                    Session[wizard.C_INSTRUCTION_INDEX] = m_job.Instructions.Count;
            }

            if (!IsPostBack)
            {
                btnNext.Attributes.Add("onClick", "javascript:HidePage();");
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);
            }

            infringementDisplay.Visible = false;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            btnBack.Click += new EventHandler(btnBack_Click);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllJobRelatedFiltered(e.Text, false);

            int itemsPerRequest = 20;
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
            string url = "wizard.aspx?step=" + step;

            if (m_isUpdate)
                url += "&jobId=" + m_jobId.ToString();

            Response.Redirect(url);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (m_isUpdate)
            {
                // Reload the job.

                Facade.IJob facJob = new Facade.Job();
                m_job = facJob.GetJob(m_jobId);

                Facade.IInstruction facInstruction = new Facade.Instruction();

                if (m_job.JobType == eJobType.Normal)
                {
                    m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                    m_job.Instructions = facInstruction.GetForJobId(m_job.JobId);
                    m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);

                    Session[wizard.C_JOB] = m_job;
                }
                else
                {
                    switch (m_job.JobType)
                    {
                        case eJobType.PalletReturn:
                            GoToStep("PR");
                            break;
                        case eJobType.Return:
                            GoToStep("GR");
                            break;
                    }
                }
            }

            GoToStep("JD");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GoToStep("CANCEL");
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int identityId = -1;

                if (cboClient.SelectedValue == "")
                {
                    // The user has not selected an organistion, they may want to create a new one.
                    pnlCreateNewOrganisation.Visible = true;

                    if (chkCreateClient.Checked)
                    {
                        Facade.IOrganisation facOrganisation = new Facade.Organisation();

                        Entities.Organisation organisation = new Entities.Organisation();
                        organisation.OrganisationName = cboClient.Text;
                        organisation.OrganisationDisplayName = cboClient.Text;
                        organisation.OrganisationType = eOrganisationType.ClientCustomer;
                        organisation.IdentityStatus = eIdentityStatus.Active;

                        string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                        Entities.FacadeResult retVal = facOrganisation.Create(organisation, userId);
                        if (!retVal.Success)
                        {
                            infringementDisplay.Infringements = retVal.Infringements;
                            infringementDisplay.DisplayInfringments();
                            infringementDisplay.Visible = true;
                        }
                        else
                            identityId = retVal.ObjectId;
                    }
                }
                else
                    identityId = Convert.ToInt32(cboClient.SelectedValue);

                if (identityId > 0)
                {
                    if (m_isUpdate)
                    {
                        if (!m_isAmendment)
                        {
                            // Set the client's customer for this point
                            m_instruction.ClientsCustomerIdentityID = identityId;

                            Session[wizard.C_INSTRUCTION] = m_instruction;
                        }

                        // Display the point being used
                        GoToStep("P");
                    }
                    else
                    {
                        if (m_isAmendment)
                        {
                            if (m_instruction.ClientsCustomerIdentityID != identityId)
                            {
                                // Remove all the point information because the client's customer has been changed
                                m_instruction.Point = null;
                                m_instruction.PointID = 0;
                                m_instruction.ClientsCustomerIdentityID = identityId;

                                Session[wizard.C_INSTRUCTION] = m_instruction;
                            }
                        }
                        else
                        {
                            // Set the client's customer for this point
                            m_instruction.ClientsCustomerIdentityID = identityId;

                            Session[wizard.C_INSTRUCTION] = m_instruction;
                        }

                        GoToStep("P");
                    }
                }
            }
        }

        #region DBCombo's Server Methods and Initialisation

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
