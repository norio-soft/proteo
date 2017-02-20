namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

    using System.Collections.Generic;

	/// <summary>
	///		Summary description for InstructionPricing.
	/// </summary>
    public partial class InstructionPricing : System.Web.UI.UserControl, IDefaultButton
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
        Entities.InstructionCollection  m_collections = new Entities.InstructionCollection();
        Entities.InstructionCollection  m_deliveries = new Entities.InstructionCollection();

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            // Retrieve the job from the session variable
            m_job = (Entities.Job)Session[wizard.C_JOB];
            m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

            // If this is an update to a job, make sure that the rates are valid.
			if (m_jobId > 0)
				m_isUpdate = true;
            else
                HandleNext();

			if (!IsPostBack)
			{
				btnNext.Attributes.Add("onClick", "javascript:HidePage();");
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                // This is a new job, so we need to enforce that rates exist.
                List<int> collectionPointIds = new List<int>();
                List<int> deliveryPointIds = new List<int>();

                #region Build the Instruction Collections

                if (m_isUpdate)
                {
                    // Populate from the legs collection
                    foreach (Entities.Instruction instruction in m_job.Instructions)
                        // Do not include point for the instruction being altered.
                        if (instruction.InstructionID != m_instruction.InstructionID)
                        {
                            switch ((eInstructionType)instruction.InstructionTypeId)
                            {
                                case eInstructionType.Load:
                                    m_collections.Add(instruction);
                                    break;
                                case eInstructionType.Drop:
                                    m_deliveries.Add(instruction);
                                    break;
                            }
                        }
                    
                }
                else
                {
                    // Populate from the instructions collection
                    foreach (Entities.Instruction instruction in m_job.Instructions)
                    {
                        switch ((eInstructionType)instruction.InstructionTypeId)
                        {
                            case eInstructionType.Load:
                                m_collections.Add(instruction);
                                break;
                            case eInstructionType.Drop:
                                m_deliveries.Add(instruction);
                                break;
                        }
                    }
                }

                // Add the current instruction if it is new.
                if (m_instruction.InstructionID == 0)
                {
                    if (m_instruction.InstructionTypeId == (int)eInstructionType.Load)
                        m_collections.Add(m_instruction);
                    else
                        m_deliveries.Add(m_instruction);
                }
                else
                {
                    // Add the point.
                    if (m_instruction.InstructionTypeId == (int)eInstructionType.Load)
                        m_collections.Add(m_instruction);
                    else
                        m_deliveries.Add(m_instruction);
                }

                #endregion

                if (m_deliveries.Count == 0)
                    HandleNext();

                foreach (Entities.Instruction collection in m_collections)
                    collectionPointIds.Add(collection.PointID);
                foreach (Entities.Instruction delivery in m_deliveries)
                    deliveryPointIds.Add(delivery.PointID);

                Facade.IJobRate facJobRate = new Facade.Job();
                DataSet dsMissingRates = facJobRate.GetMissingRates(m_job.IdentityId, collectionPointIds, deliveryPointIds);

                imgRatesRequired.Visible = false;
                if (dsMissingRates.Tables[0].Rows.Count > 0)
                {
                    repRates.DataSource = dsMissingRates;
                    repRates.DataBind();
                    lblRateAnalysis.Text = "There are rates missing, you should correct this before proceeding.";
                    chkManualRateEntry.Checked = false;
                    trRateAnalysis.Visible = true;
                    // The user can only ignore rates if the client defaults allow that option.
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    DataSet defaults = facOrganisation.GetDefaultsForIdentityId(m_job.IdentityId);
                    bool mustSpecifyRates = false;
                    try
                    {
                        mustSpecifyRates = (bool)defaults.Tables[0].Rows[0]["MustCaptureRate"];
                    }
                    catch { }
                    chkManualRateEntry.Visible = !mustSpecifyRates;
                }
                else
                    HandleNext();
			}		
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			btnBack.Click += new EventHandler(btnBack_Click);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
            repRates.ItemCommand += new RepeaterCommandEventHandler(repRates_ItemCommand);
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
			int m_collectDropIndex = 0;
			Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
			Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];

			GoToStep("PD");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void AddInstruction()
		{
			int instructionIndex = (int) Session[wizard.C_INSTRUCTION_INDEX];

			if (instructionIndex == m_job.Instructions.Count)
				m_job.Instructions.Add(m_instruction);
			else
			{
				m_job.Instructions.RemoveAt(instructionIndex);
				m_job.Instructions.Insert(instructionIndex, m_instruction);
			}
		}

		private void HandleNext()
		{
			bool success = false;

            if (m_isUpdate)
            {
                if (repRates.Items.Count == 0 || chkManualRateEntry.Checked)
                    success = UpdateInstruction();
            }
            else
            {
                AddInstruction();
                success = true;
            }

			if (success)
			{
				// Configure Session Variables
				Session[wizard.C_INSTRUCTION_INDEX] = null;
				Session[wizard.C_INSTRUCTION] = null;
				Session[wizard.C_JOB] = m_job;

				GoToStep("JD");
			}
		}

		private bool UpdateInstruction()
		{
			Facade.IJob facJob = new Facade.Job();
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			Entities.FacadeResult result = null;

			if (m_instruction.InstructionID == 0)
			{
				int plannerId = ((Entities.CustomPrincipal) Page.User).IdentityId;
				result = facJob.AddInstruction(m_job, m_instruction, plannerId, userId);
			}
			else
			{
				// Update the instruction
				result = facJob.UpdateInstruction(m_job, m_instruction, userId);
			}

			if (result.Success)
			{
                if (!chkManualRateEntry.Checked)
                {
                    // Cause the rates to be recalculated for this job.
                    facJob.PriceJob(m_jobId, ((Entities.CustomPrincipal) Page.User).UserName);
                }

				m_job = facJob.GetJob(m_jobId, true);
				m_job.Charge = ((Facade.IJobCharge) facJob).GetForJobId(m_jobId);
				m_job.References = ((Facade.IJobReference) facJob).GetJobReferences(m_jobId);               
			}

			return result.Success;
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				HandleNext();
			}
		}

        #region Rate Analysis Event Handlers

        void repRates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "setrate":
                    int collectionPointId = Convert.ToInt32(((HtmlInputHidden)e.Item.FindControl("hidCollectionPointId")).Value);
                    int deliveryPointId = Convert.ToInt32(((HtmlInputHidden)e.Item.FindControl("hidDeliveryPointId")).Value);

                    Session[wizard.C_JOB] = m_job;
                    Session[wizard.C_COLLECTION_POINT_ID] = collectionPointId;
                    Session[wizard.C_COLLECTION_POINT] = ((HtmlGenericControl)e.Item.FindControl("spnCollectionPoint")).InnerText;
                    Session[wizard.C_DELIVERY_POINT_ID] = deliveryPointId;
                    Session[wizard.C_DELIVERY_POINT] = ((HtmlGenericControl)e.Item.FindControl("spnDeliveryPoint")).InnerText;
                    Session[wizard.C_INSTRUCTION] = m_instruction;
                    Session[wizard.C_INSTRUCTION_INDEX] = Session[wizard.C_INSTRUCTION_INDEX];

                    GoToStep("AR");
                    break;
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
