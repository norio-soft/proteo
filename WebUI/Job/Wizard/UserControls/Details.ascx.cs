namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

    using System.Collections.Generic;

    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

	/// <summary>
	///		Summary description for Details.
	/// </summary>
	public partial class Details : System.Web.UI.UserControl
	{
		#region Form Elements

		protected DataGrid dgPCVs;

		#endregion

		#region Page Variables

		private         Entities.Job	m_job;
		protected       Entities.Organisation	m_client = null;
		private int		m_jobId;
        private int m_cumulativePalletCount = 0;
		private bool	m_isUpdate = false;
		private bool	m_canEdit = false;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditJob);

			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			if (Session[wizard.C_JOB] != null)
				m_job = (Entities.Job) Session[wizard.C_JOB];
			else
			{
				Facade.IJob facJob = new Facade.Job();
				m_job = facJob.GetJob(m_jobId);
				if (m_job.JobType == eJobType.Normal || m_job.JobType == eJobType.Groupage)
				{
					m_job.Charge = ((Facade.IJobCharge) facJob).GetForJobId(m_jobId);
                    Facade.IInstruction facInstruction = new Facade.Instruction();
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

			if (!IsPostBack)
			{
				using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
					m_client = facOrganisation.GetForIdentityId(m_job.IdentityId);

				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);
				btnFinish.Attributes.Add("onClick", "javascript:HidePage();");

				// Clear session variables (except the job variable)
				Session[wizard.C_CLIENT_NAME] = null;
				Session[wizard.C_COLLECT_DROP] = null;
				Session[wizard.C_COLLECT_DROP_INDEX] = null;
				Session[wizard.C_CUSTOMER_OF] = null;
				Session[wizard.C_INSTRUCTION] = null;
				Session[wizard.C_INSTRUCTION_INDEX] = null;
				Session[wizard.C_JUMP_TO_DETAILS] = false;
				Session[wizard.C_POINT_FOR] = null;
				Session[wizard.C_POINT_NAME] = null;
				Session[wizard.C_POINT_TYPE] = null;
				Session[wizard.C_TOWN_ID] = null;

				// You can only Cancel or Finish a new job, whilst you can only Close a saved job.
				btnCancel.Visible = !m_isUpdate;
				btnFinish.Visible = !m_isUpdate;
				btnClose.Visible = m_isUpdate;
			}

            // Populate the form
            PopulateJobControls();
        }

		protected void Page_Init(object sender, EventArgs e)
		{
			btnAlterJobTypeAndCharging.Click += new EventHandler(btnAlterJobTypeAndCharging_Click);
			btnAlterReferences.Click += new EventHandler(btnAlterReferences_Click);
			btnAddCollection.Click += new EventHandler(btnAddCollection_Click);
            btnAddGroupageCollection.Click += new EventHandler(btnAddCollection_Click);
			btnAddDelivery.Click += new EventHandler(btnAddDelivery_Click);

			repCollections.ItemCommand += new RepeaterCommandEventHandler(repCollections_ItemCommand);
			repCollections.ItemDataBound += new RepeaterItemEventHandler(repCollections_ItemDataBound);
			repCollections.PreRender += new EventHandler(repCollections_PreRender);

			repDeliveries.ItemCommand += new RepeaterCommandEventHandler(repDeliveries_ItemCommand);
			repDeliveries.ItemDataBound += new RepeaterItemEventHandler(repDeliveries_ItemDataBound);
			repDeliveries.PreRender += new EventHandler(repDeliveries_PreRender);

            repOrderHandling.ItemCommand += new RepeaterCommandEventHandler(repOrderHandling_ItemCommand);
            repOrderHandling.ItemDataBound += new RepeaterItemEventHandler(repOrderHandling_ItemDataBound);
            repOrderHandling.PreRender += new EventHandler(repOrderHandling_PreRender);

            repRates.ItemCommand += new RepeaterCommandEventHandler(repRates_ItemCommand);

            cfvDuplicateJobCount.ServerValidate += new ServerValidateEventHandler(cfvDuplicateJobCount_ServerValidate);

			btnFinish.Click += new EventHandler(btnFinish_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
			btnAddAnotherJob.Click += new EventHandler(btnAddAnotherJob_Click);
			btnClose.Click += new EventHandler(btnClose_Click);
            btnResourceThis.Click += new EventHandler(btnResourceThis_Click);
		}



        void cfvDuplicateJobCount_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 1, true);
        }

		#endregion

		/// <summary>
		/// Finds the instruction in the current job that has the specified instruction id.
		/// </summary>
		/// <param name="instructionId">The instruction id to use to find the instruction.</param>
		/// <returns>The instruction, or null if it is not found.</returns>
		private Entities.Instruction FindInstruction(int instructionId)
		{
		    return m_job.Instructions.GetForInstructionId(instructionId);
		}

		/// <summary>
		/// Returns all dockets that have been collected but not delivered.
		/// </summary>
		/// <returns>A collection of dockets that have been collected but not delivered.</returns>
		private Entities.CollectDropCollection GetOutstandingDockets()
		{
			Entities.CollectDropCollection dockets = new Entities.CollectDropCollection();

            foreach (Entities.Instruction instruction in m_job.Instructions)
		    {
			    if (instruction.InstructionTypeId == (int) eInstructionType.Load)
			    {
				    // Add the dockets to the collection
				    foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
				    {
					    dockets.Add(collectDrop);
				    }
			    }
			    else
			    {
				    // Find the corresponding collection dockets and remove from the collection
				    foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
				    {
					    for (int docketIndex = 0; docketIndex < dockets.Count; docketIndex++)
					    {
						    if (dockets[docketIndex].Docket == collectDrop.Docket)
						    {
							    dockets.RemoveAt(docketIndex);
							    break;
						    }
					    }
				    }
			    }
		    }

			return dockets;
		}

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		/// <summary>
		/// Populates the various parts of the pages with the relevant job data.
		/// </summary>
		private void PopulateJobControls()
		{
			#region JobId, Client, and Business Type

			if (m_jobId == 0)
			{
				lblJobId.Visible = false;
				btnAddAnotherJob.Visible = false;
                btnResourceThis.Visible = false;
			}
			else
			{
				lblJobId.Text = m_jobId.ToString();
				btnAddAnotherJob.Visible = m_job.JobType != eJobType.Groupage;
                btnResourceThis.Visible = m_job.JobType != eJobType.Groupage;
			}

			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			lblClient.Text = facOrganisation.GetForIdentityId(m_job.IdentityId).OrganisationName;
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(m_job.BusinessTypeID);
            if (businessType != null)
                lblBusinessType.Text = businessType.Description;
            else
                lblBusinessType.Text = "Business Type Not Set";

			#endregion

			#region JobType, ChargeType, and Amount

			if (m_job.IsStockMovement)
                lblJobMovement.Text = "Yes";
			else
				lblJobMovement.Text = "No";

			lblJobType.Text = Utilities.UnCamelCase(Enum.GetName(typeof(eJobType), m_job.JobType));
			lblJobChargeType.Text = Utilities.UnCamelCase(Enum.GetName(typeof(eJobChargeType), m_job.Charge.JobChargeType));
			lblJobChargeAmount.Text = m_job.Charge.JobChargeAmount.ToString("C");

			#endregion

			#region Load Number, and References

			if (m_client != null)
				lblLoadNumberText.Text = m_client.LoadNumberText;
			lblLoadNumber.Text = m_job.LoadNumber;
			repJobReferences.DataSource = m_job.References;

			#endregion

			#region PCVs, Collections, and Deliveries

			Entities.InstructionCollection collections = new Entities.InstructionCollection();
			Entities.InstructionCollection deliveries = new Entities.InstructionCollection();
            Entities.InstructionCollection orderHandlingInstructions = new Entities.InstructionCollection();

			#region Build the Instruction Collections

			// Populate from the instructions collection
			foreach (Entities.Instruction instruction in m_job.Instructions)
			{
				switch ((eInstructionType) instruction.InstructionTypeId)
				{
                    case eInstructionType.Load:
                        if (m_job.JobType == eJobType.Normal)
                            collections.Add(instruction);
                        else
                            orderHandlingInstructions.Add(instruction);
                        break;
                    case eInstructionType.Drop:
                        if (m_job.JobType == eJobType.Normal)
                            deliveries.Add(instruction);
                        else
                            orderHandlingInstructions.Add(instruction);
                        break;
                    case eInstructionType.Trunk:
                        if (m_job.JobType == eJobType.Groupage && instruction.CollectDrops.Count > 0 && instruction.CollectDrops[0].OrderID > 0)
                            orderHandlingInstructions.Add(instruction);
                        break;
                    case eInstructionType.AttemptedDelivery:
                        if (m_job.JobType == eJobType.Groupage)
                            orderHandlingInstructions.Add(instruction);
                        break;
				}
			}

			#endregion

            if (m_job.JobType == eJobType.Normal)
            {
                repCollections.DataSource = collections;
                repDeliveries.DataSource = deliveries;
            }
            else
                repOrderHandling.DataSource = orderHandlingInstructions;

			#endregion

			#region Configure Button Availability

            bool canAlterCharging = m_job.JobType != eJobType.Groupage; // Can not alter rates and type on groupage jobs.
			bool canAlterReferences = true;
			bool canAddCollection = true;
			bool canAddDelivery = true;
            pnlNormalJob.Visible = m_job.JobType == eJobType.Normal;
            pnlGroupageJob.Visible = m_job.JobType == eJobType.Groupage;

            Facade.IJob facJob = new Facade.Job();
			if (m_job.JobState == eJobState.Cancelled || m_job.ForCancellation || m_job.HasBeenPosted || facJob.IsBeingInvoiced(m_job.JobId))
			{
				// Don't allow anything to be changed
				canAlterCharging = false;
				canAlterReferences = false;
				canAddCollection = false;
				canAddDelivery = false;
                m_canEdit = false;
			}
            else if (m_job.IsPriced)
            {
                // Don't allow the charging to be changed or new collections or deliveries to be added
                canAlterCharging = false;
                canAddCollection = false;
                canAddDelivery = false;
                m_canEdit = false;
            }
            else if (m_job.JobState == eJobState.Completed || m_job.JobState == eJobState.BookingInComplete || m_job.JobState == eJobState.BookingInIncomplete)
            {
                // Don't allow new collections or deliveries to be added
                canAddCollection = false;
                canAddDelivery = false;
                m_canEdit = false;
            }

			btnAlterJobTypeAndCharging.Enabled = m_canEdit && canAlterCharging;
			btnAlterReferences.Enabled = m_canEdit && canAlterReferences;
			btnAddCollection.Enabled = m_canEdit && canAddCollection;
            btnAddGroupageCollection.Enabled = m_canEdit && canAddCollection;
			btnAddDelivery.Enabled = m_canEdit && canAddDelivery;

			// Can only add delivery if there are outstanding dockets.
			btnAddDelivery.Enabled = m_canEdit && GetOutstandingDockets().Count > 0;

			#endregion

            trRateAnalysis.Visible = false;
            if (!m_isUpdate && deliveries.Count > 0)
            {
                // This is a new job, so we need to enforce that rates exist.
                List<int> collectionPointIds = new List<int>();
                List<int> deliveryPointIds = new List<int>();

                foreach (Entities.Instruction collection in collections)
                    collectionPointIds.Add(collection.PointID);
                foreach (Entities.Instruction delivery in deliveries)
                    deliveryPointIds.Add(delivery.PointID);

                Facade.IJobRate facJobRate = new Facade.Job();
                DataSet dsMissingRates = facJobRate.GetMissingRates(m_job.IdentityId, collectionPointIds, deliveryPointIds);

                imgRatesRequired.Visible = false;
                if (dsMissingRates.Tables[0].Rows.Count > 0)
                {
                    repRates.DataSource = dsMissingRates;
                    lblRateAnalysis.Text = "There are rates missing, you should correct this before creating the job.";
                    chkManualRateEntry.Checked = false;
                    trRateAnalysis.Visible = true;
                    // The user can only ignore rates if the client defaults allow that option.
                    DataSet defaults = facOrganisation.GetDefaultsForIdentityId(m_job.IdentityId);
                    bool mustSpecifyRates = false;
                    try
                    {
                        mustSpecifyRates = (bool)defaults.Tables[0].Rows[0]["MustCaptureRate"];
                    }
                    catch { }
                    chkManualRateEntry.Visible = !mustSpecifyRates;
                }
            }

			Page.DataBind();

            if (!IsPostBack)
            {
                #region Control Area

                using (Facade.IControlArea facControlArea = new Facade.Traffic())
                    cboControlArea.DataSource = facControlArea.GetAll();
                cboControlArea.DataBind();

                if (m_isUpdate)
                {
                    cboControlArea.Enabled = false;
                    cboControlArea.ClearSelection();
                    cboControlArea.Items.FindByValue(m_job.ControlAreaId.ToString()).Selected = true;
                }
                else
                {
                    cboControlArea.Items.Insert(0, new ListItem("Auto-Detect", "0"));
                    cboControlArea.Enabled = true;
                    cboControlArea.ClearSelection();
                    try
                    {
                        cboControlArea.Items.FindByValue(collections[0].Point.Address.TrafficArea.ControlAreaId.ToString()).Selected = true;
                    }
                    catch
                    {
                        cboControlArea.Items[0].Selected = true;
                    }
                }

                #endregion

                #region Duplicate Job Count

                if (m_isUpdate)
                    trDuplicateJobs.Visible = false;
                else
                    txtDuplicateJobCount.Text = "1";

                #endregion
            }

            bool canBeFinished = ValidateJobCanBeFinished();
			btnFinish.Enabled = canBeFinished;
		}

		/// <summary>
		/// Validates that the job is in a suitable state for creating.
		/// </summary>
		/// <returns>True if the job can be finished, False otherwise.</returns>
		private bool ValidateJobCanBeFinished()
		{
			bool canBeFinished = true;
			Entities.InstructionCollection collections = (Entities.InstructionCollection) repCollections.DataSource;
			Entities.InstructionCollection deliveries = (Entities.InstructionCollection) repDeliveries.DataSource;
            Entities.InstructionCollection orderHandlingInstructions = (Entities.InstructionCollection)repOrderHandling.DataSource;

            // The job must have at least one collection.
            if (m_job.JobType == eJobType.Normal)
            {
                if (collections.Count == 0)
                    canBeFinished = false;
            }
            else
                if (orderHandlingInstructions.Count == 0)
                    canBeFinished = false;

			return canBeFinished;
		}

		#region Button Event Handlers

		private void btnAddAnotherJob_Click(object sender, EventArgs e)
		{
			Response.Redirect("wizard.aspx");
		}

		private void btnAlterJobTypeAndCharging_Click(object sender, EventArgs e)
		{
			Session[wizard.C_JUMP_TO_DETAILS] = true;

			GoToStep("JT");
		}

		private void btnAlterReferences_Click(object sender, EventArgs e)
		{
			Session[wizard.C_JUMP_TO_DETAILS] = true;

			GoToStep("JR");
		}

		private void btnAddCollection_Click(object sender, EventArgs e)
		{
			Session[wizard.C_JUMP_TO_DETAILS] = true;
			Session[wizard.C_POINT_TYPE] = ePointType.Collect;

            if (m_job.JobType == eJobType.Groupage)
                GoToStep("SGO");
            else
			    GoToStep("P");
		}

		private void btnAddDelivery_Click(object sender, EventArgs e)
		{
			Session[wizard.C_JUMP_TO_DETAILS] = true;
			Session[wizard.C_POINT_TYPE] = ePointType.Deliver;
			
			GoToStep("CC");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
            GoToStep("CANCEL");
		}

		private void btnFinish_Click(object sender, EventArgs e)
		{
            if (Page.IsValid)
            {
                // If this is a new job create it.
                if (!m_isUpdate)
                {
                    if (repRates.Items.Count == 0 || chkManualRateEntry.Checked)
                    {
                        var userName = ((Entities.CustomPrincipal)Page.User).UserName;

                        Facade.IJob facJob = new Facade.Job();
                        m_job.ControlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);

                        // Create the jobs the required number of times.
                        int requiredJobCount = int.Parse(txtDuplicateJobCount.Text);

                        if (requiredJobCount == 1)
                            m_jobId = facJob.Create(m_job, !chkManualRateEntry.Checked, userName).ObjectId;
                        else
                        {
                            // Create the job template.
                            BinaryFormatter formatter = new BinaryFormatter();
                            MemoryStream ms = new MemoryStream();
                            formatter.Serialize(ms, m_job);

                            for (int i = 0; i < requiredJobCount; i++)
                            {
                                m_jobId = facJob.Create(m_job, !chkManualRateEntry.Checked, userName).ObjectId;

                                ms.Position = 0;
                                try
                                {
                                    m_job = (Entities.Job)formatter.Deserialize(ms);
                                }
                                catch (Exception excp)
                                {
                                    m_jobId = 0;
                                }
                            }
                        }

                        if (m_jobId > 0)
                        {
                            // The job was successfully created
                            Session[wizard.C_JOB] = null;

                            // Redisplay this page, but in an update mode
                            m_isUpdate = true;
                            GoToStep("JD");
                        }
                        else
                        {
                            // The job was not created!
                            trFailed.Visible = true;
                        }
                    }
                    else
                    {
                        // The user needs to configure some rates before creating the job.
                        imgRatesRequired.Visible = true;
                    }
                }
            }
		}

        void btnResourceThis_Click(object sender, EventArgs e)
        {
            if (m_job != null)
            {
                Entities.LegPlan legPlan = new Facade.Instruction().GetLegPlan(m_job.Instructions, false);
                Entities.LegView leg = legPlan.Legs()[0];
                string command = "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../../traffic/resourceThis.aspx?iID=" + leg.InstructionID + "&Driver=&DR=&RegNo=&VR=&TrailerRef=&TR=&LS=" + leg.StartLegPoint.PlannedDateTime + "&LE=" + leg.EndLegPoint.PlannedDateTime + "&DC=&CA=" + m_job.ControlAreaId + "&TA=" + m_job.TrafficAreaId + "&LastUpdateDate=" + m_job.LastUpdateDate + "&jobId=" + m_jobId + "&depotId=0") + "', 'ResourceThis', 'width=500,height=400,resizable=no,scrollbars=Yes');</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", command);
            }
        }

		#endregion

		#region Collection Event Handlers

		private void repCollections_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			int plannerId = ((Entities.CustomPrincipal) Page.User).IdentityId;

			switch (e.CommandName.ToLower())
			{
				case "down":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int followingInstructionId = Int32.Parse(((HtmlInputHidden) repCollections.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.UpdateSwitchActions(m_job, followingInstructionId, selectedInstructionId, plannerId, userId);

						m_job = facJob.GetJob(m_jobId, true, true);
						m_job.Charge = ((Facade.IJobCharge) facJob).GetForJobId(m_jobId);
						m_job.References = ((Facade.IJobReference) facJob).GetJobReferences(m_jobId);
					}
					else
					{
						int selectedInstructionRepeaterItemIndex = e.Item.ItemIndex + 1;
						int followingInstructionRepeaterItemIndex = selectedInstructionRepeaterItemIndex + 1;

						int collectionCounter = 0;

						int selectedInstructionEntityIndex = -1;
						int followingInstructionEntityIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Load)
							{
								collectionCounter++;

								if (collectionCounter == selectedInstructionRepeaterItemIndex)
								{
									selectedInstructionEntityIndex = i;
								}
								else if (collectionCounter == followingInstructionRepeaterItemIndex)
								{
									followingInstructionEntityIndex = i;
								}
							}
						}

						if (selectedInstructionEntityIndex > -1 && followingInstructionEntityIndex > -1)
						{
							Entities.Instruction movingInstruction = m_job.Instructions[selectedInstructionEntityIndex];
							m_job.Instructions.RemoveAt(selectedInstructionEntityIndex);
							m_job.Instructions.Insert(followingInstructionEntityIndex, movingInstruction);
						}					
					}

					Session[wizard.C_JOB] = m_job;
					GoToStep("JD");
					break;
				case "up":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int previousInstructionId = Int32.Parse(((HtmlInputHidden) repCollections.Items[e.Item.ItemIndex - 1].FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.UpdateSwitchActions(m_job, selectedInstructionId, previousInstructionId, plannerId, userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                    }
					else
					{
						int selectedInstructionRepeaterItemIndex = e.Item.ItemIndex + 1;
						int preceedingInstructionRepeaterItemIndex = selectedInstructionRepeaterItemIndex - 1;

						int collectionCounter = 0;

						int selectedInstructionEntityIndex = -1;
						int preceedingInstructionEntityIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Load)
							{
								collectionCounter++;

								if (collectionCounter == selectedInstructionRepeaterItemIndex)
								{
									selectedInstructionEntityIndex = i;
								}
								else if (collectionCounter == preceedingInstructionRepeaterItemIndex)
								{
									preceedingInstructionEntityIndex = i;
								}
							}
						}

						if (selectedInstructionEntityIndex > -1 && preceedingInstructionEntityIndex > -1)
						{
							Entities.Instruction movingInstruction = m_job.Instructions[selectedInstructionEntityIndex];
							m_job.Instructions.RemoveAt(selectedInstructionEntityIndex);
							m_job.Instructions.Insert(preceedingInstructionEntityIndex, movingInstruction);
						}					
					}

					Session[wizard.C_JOB] = m_job;
					GoToStep("JD");
					break;
				case "alter":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);

						Entities.Instruction selectedInstruction = FindInstruction(selectedInstructionId);

						if (selectedInstruction != null)
						{
							Session[wizard.C_INSTRUCTION] = selectedInstruction;
							Session[wizard.C_JUMP_TO_DETAILS] = true;
							Session[wizard.C_COLLECT_DROP_INDEX] = 0;

                            if (m_job.JobType == eJobType.Normal)
                                GoToStep("P");
                            else
                                GoToStep("SGO");
						}
					}
					else
					{
						int collectionNumber = e.Item.ItemIndex + 1;
						int collectionCounter = 0;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Load)
							{
								collectionCounter++;

								if (collectionCounter == collectionNumber)
								{
									Session[wizard.C_INSTRUCTION] = instruction;
									Session[wizard.C_INSTRUCTION_INDEX] = i;
								}
							}
						}

						Session[wizard.C_JUMP_TO_DETAILS] = true;
						Session[wizard.C_COLLECT_DROP_INDEX] = 0;

						GoToStep("P");
					}
					break;
				case "delete":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.RemoveInstruction(m_job, selectedInstructionId, userId);

                        // Cause the rates to be recalculated for this job.
                        facJob.PriceJob(m_jobId, ((Entities.CustomPrincipal)Page.User).UserName);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                        Session[wizard.C_JOB] = m_job;
						GoToStep("JD");
					}
					else
					{
						int collectionNumber = e.Item.ItemIndex + 1;
						int collectionCounter = 0;
						int collectionIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Load)
							{
								collectionCounter++;

								if (collectionCounter == collectionNumber)
								{
									collectionIndex = i;
								}
							}						
						}

						m_job.Instructions.RemoveAt(collectionIndex);
						Session[wizard.C_JOB] = m_job;
						GoToStep("JD");
					}
					break;
			}
		}

		private void repCollections_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				Entities.Instruction instruction = (Entities.Instruction) e.Item.DataItem;

				Label lblBookedDateTime = (Label) e.Item.FindControl("lblBookedDateTime");

				if (instruction.IsAnyTime)
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " AnyTime";
				else
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");

				if (instruction.Note.Trim().Length == 0)
					((HtmlTableRow) e.Item.FindControl("rowInstructionNote")).Visible = false;
			}		
		}

		private void repCollections_PreRender(object sender, EventArgs e)
		{
			int itemIndex;

			Facade.IInstructionActual facInstructionActual = new Facade.Instruction();

			foreach (RepeaterItem item in repCollections.Items)
			{
				if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
				{
					bool allowUp = false;
					bool allowDown = false;
					bool allowAlter = false;
					bool allowDelete = false;
					bool hasActual = false;

					if (m_canEdit)
					{
						itemIndex = item.ItemIndex;
						int instructionId = 0;

						// See if there is an actual for this instruction (only possible if the job has previously been created).
						if (m_isUpdate)
						{
							HtmlInputHidden hidInstructionId = (HtmlInputHidden) item.FindControl("hidInstructionId");
							instructionId = Convert.ToInt32(hidInstructionId.Value);

							Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
							hasActual = actual != null;
						}
						else
							hasActual = false;

						// Instructions can be moved if the instruction and the move target have no actuals
						if (itemIndex > 0 && !hasActual)
						{
							int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repCollections.Items[itemIndex - 1].FindControl("hidInstructionId")).Value);
							bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

							if (!previousHasActual)
								allowUp = true;
						}

						if (itemIndex < repCollections.Items.Count - 1 && !hasActual)
							allowDown = true;

						// Instructions can be edited unless that instruction has an actual recorded.
						allowAlter = !hasActual;

						// Instructions can be deleted unless it is the only collection or has an actual recorded.
						if (repCollections.Items.Count == 1 || hasActual)
							allowDelete = false;
						else
							allowDelete = true;
					}

					((Button) item.FindControl("btnUp")).Enabled = allowUp;
					((Button) item.FindControl("btnUp")).Attributes.Add("onClick", "javascript:HidePage();");
					((Button) item.FindControl("btnDown")).Enabled = allowDown;
					((Button) item.FindControl("btnDown")).Attributes.Add("onClick", "javascript:HidePage();");
					((Button) item.FindControl("btnAlter")).Enabled = allowAlter;
					((Button) item.FindControl("btnDelete")).Enabled = allowDelete;
					((Button) item.FindControl("btnDelete")).Attributes.Add("onClick", "javascript:HidePage();");
				}
			}
		}

		#endregion

		#region Delivery Event Handlers

		private void repDeliveries_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			int plannerId = ((Entities.CustomPrincipal) Page.User).IdentityId;

			switch (e.CommandName.ToLower())
			{
				case "down":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int followingInstructionId = Int32.Parse(((HtmlInputHidden) repDeliveries.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.UpdateSwitchActions(m_job, followingInstructionId, selectedInstructionId, plannerId, userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                    }
					else
					{
						int selectedInstructionRepeaterItemIndex = e.Item.ItemIndex + 1;
						int followingInstructionRepeaterItemIndex = selectedInstructionRepeaterItemIndex + 1;

						int deliveryCounter = 0;

						int selectedInstructionEntityIndex = -1;
						int followingInstructionEntityIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
							{
								deliveryCounter++;

								if (deliveryCounter == selectedInstructionRepeaterItemIndex)
								{
									selectedInstructionEntityIndex = i;
								}
								else if (deliveryCounter == followingInstructionRepeaterItemIndex)
								{
									followingInstructionEntityIndex = i;
								}
							}
						}

						if (selectedInstructionEntityIndex > -1 && followingInstructionEntityIndex > -1)
						{
							Entities.Instruction movingInstruction = m_job.Instructions[selectedInstructionEntityIndex];
							m_job.Instructions.RemoveAt(selectedInstructionEntityIndex);
							m_job.Instructions.Insert(followingInstructionEntityIndex, movingInstruction);
						}
					}

					Session[wizard.C_JOB] = m_job;
					GoToStep("JD");
					break;
				case "up":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int previousInstructionId = Int32.Parse(((HtmlInputHidden) repDeliveries.Items[e.Item.ItemIndex - 1].FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.UpdateSwitchActions(m_job, selectedInstructionId, previousInstructionId, plannerId, userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                    }
					else
					{
						int selectedInstructionRepeaterItemIndex = e.Item.ItemIndex + 1;
						int preceedingInstructionRepeaterItemIndex = selectedInstructionRepeaterItemIndex - 1;

						int deliveryCounter = 0;

						int selectedInstructionEntityIndex = -1;
						int preceedingInstructionEntityIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
							{
								deliveryCounter++;

								if (deliveryCounter == selectedInstructionRepeaterItemIndex)
								{
									selectedInstructionEntityIndex = i;
								}
								else if (deliveryCounter == preceedingInstructionRepeaterItemIndex)
								{
									preceedingInstructionEntityIndex = i;
								}
							}
						}

						if (selectedInstructionEntityIndex > -1 && preceedingInstructionEntityIndex > -1)
						{
							Entities.Instruction movingInstruction = m_job.Instructions[selectedInstructionEntityIndex];
							m_job.Instructions.RemoveAt(selectedInstructionEntityIndex);
							m_job.Instructions.Insert(preceedingInstructionEntityIndex, movingInstruction);
						}
					}

					Session[wizard.C_JOB] = m_job;
					GoToStep("JD");
					break;
				case "alter":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);

						Entities.Instruction selectedInstruction = FindInstruction(selectedInstructionId);

						if (selectedInstruction != null)
						{
							Session[wizard.C_INSTRUCTION] = selectedInstruction;
							Session[wizard.C_JUMP_TO_DETAILS] = true;
							Session[wizard.C_COLLECT_DROP_INDEX] = 0;

                            if (m_job.JobType == eJobType.Normal)
                                GoToStep("P");
                            else
                                GoToStep("SGO");
						}
					}
					else
					{
						int deliveryNumber = e.Item.ItemIndex + 1;
						int deliveryCounter = 0;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
							{
								deliveryCounter++;

								if (deliveryCounter == deliveryNumber)
								{
									Session[wizard.C_INSTRUCTION] = instruction;
									Session[wizard.C_INSTRUCTION_INDEX] = i;
								}
							}
						}

						Session[wizard.C_JUMP_TO_DETAILS] = true;
						Session[wizard.C_COLLECT_DROP_INDEX] = 0;

						GoToStep("P");
					}
					break;
				case "delete":
					if (m_isUpdate)
					{
						int selectedInstructionId = Int32.Parse(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);

						Facade.IJob facJob = new Facade.Job();
						facJob.RemoveInstruction(m_job, selectedInstructionId, userId);

                        // Cause the rates to be recalculated for this job.
                        facJob.PriceJob(m_jobId, ((Entities.CustomPrincipal)Page.User).UserName);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                        Session[wizard.C_JOB] = m_job;
						GoToStep("JD");
					}
					else
					{
						int deliveryNumber = e.Item.ItemIndex + 1;
						int deliveryCounter = 0;
						int deliveryIndex = -1;

						for (int i = 0; i < m_job.Instructions.Count; i++)
						{
							Entities.Instruction instruction = m_job.Instructions[i];

							if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
							{
								deliveryCounter++;

								if (deliveryCounter == deliveryNumber)
								{
									deliveryIndex = i;
								}
							}						
						}

						m_job.Instructions.RemoveAt(deliveryIndex);
						Session[wizard.C_JOB] = m_job;
						GoToStep("JD");
					}
					break;
			}		
		}

		private void repDeliveries_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				Entities.Instruction instruction = (Entities.Instruction) e.Item.DataItem;

				Label lblClientsCustomer = (Label) e.Item.FindControl("lblClientsCustomer");
				Label lblBookedDateTime = (Label) e.Item.FindControl("lblBookedDateTime");

				// Display the client's customer
				Facade.IOrganisation facOrganisation = new Facade.Organisation();
				lblClientsCustomer.Text = facOrganisation.GetForIdentityId(instruction.ClientsCustomerIdentityID).OrganisationName;

				// Display the booked date and time
				if (instruction.IsAnyTime)
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " AnyTime";
				else
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");

				if (instruction.Note.Trim().Length == 0)
					((HtmlTableRow) e.Item.FindControl("rowInstructionNote")).Visible = false;
			}		
		}

		private void repDeliveries_PreRender(object sender, EventArgs e)
		{
			int itemIndex;

			Facade.IInstructionActual facInstructionActual = new Facade.Instruction();

			foreach (RepeaterItem item in repDeliveries.Items)
			{
				if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
				{
					bool allowUp = false;
					bool allowDown = false;
					bool allowAlter = false;
					bool allowDelete = false;
					bool hasActual = false;

					if (m_canEdit)
					{
						itemIndex = item.ItemIndex;
						int instructionId = 0;

						// See if there is an actual for this instruction (only possible if the job has previously been created.
						if (m_isUpdate)
						{
							HtmlInputHidden hidInstructionId = (HtmlInputHidden) item.FindControl("hidInstructionId");
							instructionId = Convert.ToInt32(hidInstructionId.Value);

							Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
							hasActual = actual != null;
						}
						else
							hasActual = false;

						// Instructions can be moved if the instruction and the move target have no actuals
						if (itemIndex > 0 && !hasActual)
						{
							int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repDeliveries.Items[itemIndex - 1].FindControl("hidInstructionId")).Value);
							bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

							if (!previousHasActual)
								allowUp = true;
						}

						if (itemIndex < repDeliveries.Items.Count - 1 && !hasActual)
							allowDown = true;

						// Instructions can be edited unless that instruction has an actual recorded.
						allowAlter = !hasActual;

						// Instructions can be deleted unless that instruction has an actual recorded.
						allowDelete = !hasActual;

						// Can only add more collections if no deliveries have actuals recorded.
						if (btnAddCollection.Enabled && hasActual)
							btnAddCollection.Enabled = false;
					}

					((Button) item.FindControl("btnUp")).Enabled = allowUp;
					((Button) item.FindControl("btnUp")).Attributes.Add("onClick", "javascript:HidePage();");
					((Button) item.FindControl("btnDown")).Enabled = allowDown;
					((Button) item.FindControl("btnDown")).Attributes.Add("onClick", "javascript:HidePage();");
					((Button) item.FindControl("btnAlter")).Enabled = allowAlter;
					((Button) item.FindControl("btnDelete")).Enabled = allowDelete;
					((Button) item.FindControl("btnDelete")).Attributes.Add("onClick", "javascript:HidePage();");
				}
			}
		}

		#endregion

        #region Order Handling Event Handlers

        void repOrderHandling_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            int plannerId = ((Entities.CustomPrincipal)Page.User).IdentityId;

            switch (e.CommandName.ToLower())
            {
                case "down":
                    if (m_isUpdate)
                    {
                        int selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                        int followingInstructionId = Int32.Parse(((HtmlInputHidden)repOrderHandling.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId")).Value);

                        Facade.IJob facJob = new Facade.Job();
                        facJob.UpdateSwitchActions(m_job, followingInstructionId, selectedInstructionId, plannerId, userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                    }
                    else
                    {

                    }

                    Session[wizard.C_JOB] = m_job;
                    GoToStep("JD");
                    break;
                case "up":
                    if (m_isUpdate)
                    {
                        int selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                        int previousInstructionId = Int32.Parse(((HtmlInputHidden)repOrderHandling.Items[e.Item.ItemIndex - 1].FindControl("hidInstructionId")).Value);

                        Facade.IJob facJob = new Facade.Job();
                        facJob.UpdateSwitchActions(m_job, selectedInstructionId, previousInstructionId, plannerId, userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                    }
                    else
                    {

                    }

                    Session[wizard.C_JOB] = m_job;
                    GoToStep("JD");
                    break;
                case "merge":
                    if (m_isUpdate)
                    {
                        int selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                        int followingInstructionId = int.Parse((repOrderHandling.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId") as HtmlInputHidden).Value);

                        Facade.IJob facJob = new Facade.Job();
                        facJob.MergeInstructions(
                            m_job.Instructions.GetForInstructionId(selectedInstructionId),
                            m_job.Instructions.GetForInstructionId(followingInstructionId),
                            userId);

                        m_job = facJob.GetJob(m_jobId, true, true);
                        m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                        m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                        Session[wizard.C_JOB] = m_job;
                        GoToStep("JD");
                    }
                    else
                    {

                    }
                    break;
                case "alter":
                    if (m_isUpdate)
                    {
                        int selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);

                        Entities.Instruction selectedInstruction = FindInstruction(selectedInstructionId);

                        if (selectedInstruction != null)
                        {
                            Session[wizard.C_INSTRUCTION] = selectedInstruction;
                            Session[wizard.C_JUMP_TO_DETAILS] = true;
                            Session[wizard.C_COLLECT_DROP_INDEX] = 0;

                            // If this collection has been called just allow the user to change what happens to the dockets, otherwise allow collection changes.
                            if (selectedInstruction.InstructionActuals == null || selectedInstruction.InstructionActuals.Count == 0)
                                GoToStep("SGO");
                            else
                            {
                                Session[wizard.C_ADDED_ORDERS] = new List<int>();
                                Session[wizard.C_REMOVED_ORDERS] = new List<int>();

                                GoToStep("SGOH");
                            }
                        }
                    }
                    else
                    {

                    }
                    break;
            }
        }

        void repOrderHandling_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.Instruction instruction = (Entities.Instruction)e.Item.DataItem;

                if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                    m_cumulativePalletCount += instruction.TotalPallets;
                else if (instruction.InstructionTypeId == (int)eInstructionType.AttemptedDelivery)
                {
                    // Do nothing!
                }
                else
                    m_cumulativePalletCount -= instruction.TotalPallets;

                Label lblInstructionType = e.Item.FindControl("lblInstructionType") as Label;
                Label lblBookedDateTime = (Label)e.Item.FindControl("lblBookedDateTime");
                Label lblPalletsOn = e.Item.FindControl("lblPalletsOn") as Label;

                // Display the action being performed.
                if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                    lblInstructionType.Text = "Collect";
                else if (instruction.InstructionTypeId == (int)eInstructionType.AttemptedDelivery)
                    lblInstructionType.Text = "Attempted Delivery";
                else
                    lblInstructionType.Text = "Drop";
                
                // Display the booked date and time
                if (instruction.IsAnyTime)
                    lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " AnyTime";
                else
                    lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");

                // Display the cumulative number of pallets on the trailer after this instruction is performed.
                lblPalletsOn.Text = m_cumulativePalletCount.ToString();
            }
        }

        void repOrderHandling_PreRender(object sender, EventArgs e)
        {
            int itemIndex;

            Facade.IInstructionActual facInstructionActual = new Facade.Instruction();

            foreach (RepeaterItem item in repOrderHandling.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
                {
                    bool allowUp = false;
                    bool allowDown = false;
                    bool allowMerge = false;
                    bool allowAlter = false;
                    bool hasActual = false;

                    eInstructionType instructionType = (eInstructionType)int.Parse((item.FindControl("hidInstructionTypeId") as HtmlInputHidden).Value);

                    if (m_canEdit && instructionType != eInstructionType.AttemptedDelivery)
                    {
                        itemIndex = item.ItemIndex;
                        int instructionId = 0;

                        // See if there is an actual for this instruction (only possible if the job has previously been created.
                        if (m_isUpdate)
                        {
                            HtmlInputHidden hidInstructionId = (HtmlInputHidden)item.FindControl("hidInstructionId");
                            instructionId = Convert.ToInt32(hidInstructionId.Value);

                            Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
                            hasActual = actual != null;
                        }
                        else
                            hasActual = false;

                        Entities.Instruction instruction = null;
                        if (m_isUpdate)
                            instruction = (facInstructionActual as Facade.IInstruction).GetInstruction(instructionId);
                        else
                            instruction = m_job.Instructions[itemIndex];

                        // Instructions can be moved if the instruction and the move target have no actuals and the previous instruction is not
                        // loading goods this instruction is delivering.
                        if (itemIndex > 0 && !hasActual)
                        {
                            int previousInstructionId = Convert.ToInt32(((HtmlInputHidden)repOrderHandling.Items[itemIndex - 1].FindControl("hidInstructionId")).Value);
                            eInstructionType previousInstructionType = (eInstructionType)int.Parse((repOrderHandling.Items[itemIndex - 1].FindControl("hidInstructionTypeId") as HtmlInputHidden).Value);

                            bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

                            if (previousInstructionType != eInstructionType.AttemptedDelivery)
                            {
                                if (previousInstructionType != eInstructionType.Load)
                                    allowUp = true;
                                else
                                {
                                    bool found = false;
                                    Entities.Instruction previousInstruction = null;
                                    if (m_isUpdate)
                                        previousInstruction = (facInstructionActual as Facade.IInstruction).GetInstruction(previousInstructionId);
                                    else
                                        previousInstruction = m_job.Instructions[itemIndex - 1];
                                    foreach (Entities.CollectDrop previousCD in previousInstruction.CollectDrops)
                                        if (instruction.CollectDrops.GetForOrderID(previousCD.OrderID) != null)
                                            found = true;

                                    allowUp = !found;
                                }
                            }
                        }

                        if (itemIndex < repOrderHandling.Items.Count - 1 && !hasActual)
                        {
                            int followingInstructionId = Convert.ToInt32(((HtmlInputHidden)repOrderHandling.Items[itemIndex + 1].FindControl("hidInstructionId")).Value);
                            eInstructionType followingInstructionType = (eInstructionType)int.Parse((repOrderHandling.Items[itemIndex + 1].FindControl("hidInstructionTypeId") as HtmlInputHidden).Value);
                            Entities.Instruction followingInstruction = null;
                            if (m_isUpdate)
                                followingInstruction = (facInstructionActual as Facade.IInstruction).GetInstruction(followingInstructionId);
                            else
                                followingInstruction = m_job.Instructions[itemIndex + 1];

                            if (followingInstructionType != eInstructionType.AttemptedDelivery)
                            {
                                // Instructions can be moved down if the following instruction is not delivering goods this instruction is loading.
                                if (followingInstructionType != eInstructionType.Drop && followingInstructionType != eInstructionType.Trunk)
                                    allowDown = true;
                                else
                                {
                                    bool found = false;
                                    foreach (Entities.CollectDrop followingCD in followingInstruction.CollectDrops)
                                        if (instruction.CollectDrops.GetForOrderID(followingCD.OrderID) != null)
                                            found = true;

                                    allowDown = !found;
                                }

                                // Instructions can be merged if the following instruction is of the same type and this instruction does not have an actual and both instructions occur at the same point.
                                allowMerge = instructionType == followingInstructionType && instruction.PointID == followingInstruction.PointID;
                            }
                        }

                        // Instructions can be edited if it is a collection and not all it's delivery dockets have been called in.
                        if (!m_isUpdate)
                            allowAlter = instructionType == eInstructionType.Load;
                        else
                        {
                            Facade.IOrder facOrder = new Facade.Order();
                            DataSet dsOrderHandling = facOrder.GetOrdersForInstructionID(instructionId);
                            allowAlter = instructionType == eInstructionType.Load && !hasActual;
                            //allowAlter = instructionType == eInstructionType.Load && dsOrderHandling.Tables[0].Select("DeliveryCalledIn = 0").Length > 0;
                        }

                        btnAddCollection.Enabled = m_job.JobState == eJobState.Booked || m_job.JobState == eJobState.Planned || m_job.JobState == eJobState.InProgress;
                    }

                    ((Button)item.FindControl("btnUp")).Enabled = allowUp;
                    ((Button)item.FindControl("btnUp")).Attributes.Add("onClick", "javascript:HidePage();");
                    ((Button)item.FindControl("btnDown")).Enabled = allowDown;
                    ((Button)item.FindControl("btnDown")).Attributes.Add("onClick", "javascript:HidePage();");
                    ((Button)item.FindControl("btnMerge")).Enabled = allowMerge;
                    ((Button)item.FindControl("btnMerge")).Attributes.Add("onClick", "javascript:HidePage();");
                    ((Button)item.FindControl("btnAlter")).Enabled = allowAlter;
                }
            }
        }

        protected void repOrderCollectDrops_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.CollectDrop collectDrop = (Entities.CollectDrop)e.Item.DataItem;
                Label lblOrderAction = e.Item.FindControl("lblOrderAction") as Label;
                Label lblWeightCode = e.Item.FindControl("lblWeightCode") as Label;

                eInstructionType instructionType = (eInstructionType)int.Parse(((sender as Repeater).Parent.FindControl("hidInstructionTypeId") as HtmlInputHidden).Value);

                if (instructionType == eInstructionType.Load)
                    lblOrderAction.Text = "Collect";
                else if (instructionType == eInstructionType.Drop)
                    lblOrderAction.Text = "Deliver";
                else
                    lblOrderAction.Text = collectDrop.OrderAction.ToString().Replace("_", " ");

                lblWeightCode.Text = Facade.WeightType.GetForWeightTypeId(collectDrop.Order.WeightTypeID).ShortCode;
            }
        }

        #endregion

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

		}
		#endregion
	}
}
