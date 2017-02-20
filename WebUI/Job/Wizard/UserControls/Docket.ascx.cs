namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	
	/// <summary>
	///		Summary description for Docket.
	/// </summary>
    public partial class Docket : System.Web.UI.UserControl, IDefaultButton
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

		private Entities.Job			m_job;
		private int						m_jobId;
		private bool					m_isUpdate = false;
		private bool					m_isAmendment = false;

		private Entities.Instruction	m_instruction;
		private Entities.CollectDrop	m_collectdrop;
		
		private int						m_collectDropIndex;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			m_job = (Entities.Job) Session[wizard.C_JOB];
			m_instruction = (Entities.Instruction) Session[wizard.C_INSTRUCTION];

			if (Session[wizard.C_COLLECT_DROP_INDEX] != null)
			{
				m_collectDropIndex = (int) Session[wizard.C_COLLECT_DROP_INDEX];

				if (m_collectDropIndex != m_instruction.CollectDrops.Count)
				{
					m_isAmendment = true;

					// You can only add a docket at the end
					if (m_collectDropIndex < m_instruction.CollectDrops.Count - 1)
                        chkAddAnotherDocket.Visible = false;
				}
			}

			if (!IsPostBack)
			{
				btnNext.Attributes.Add("onClick", "javascript:HidePage();");
				btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                

                // Do not show the remove docket button for the only docket in each collection/drop - they must delete the instruction to
                // remove all dockets.
                btnRemoveDocket.Visible = m_instruction.CollectDrops.Count > 1 || m_collectDropIndex == m_instruction.CollectDrops.Count;

				ConfigureDocketTerminology();

				// Details of added Dockets
				repDockets.DataSource = m_instruction.CollectDrops;
				repDockets.DataBind();

                dteBookedDate.SelectedDate = m_instruction.BookedDateTime;
				if (m_instruction.IsAnyTime)
					dteBookedTime.Text = "AnyTime";
				else
                    dteBookedTime.SelectedDate = m_instruction.BookedDateTime;

				if (Session[wizard.C_COLLECT_DROP] != null)
				{
					m_collectdrop = (Entities.CollectDrop) Session[wizard.C_COLLECT_DROP];
				}
				else
				{
					m_collectdrop = new Entities.CollectDrop();
                    m_collectdrop.InstructionID = m_instruction.InstructionID;

					Session[wizard.C_COLLECT_DROP] = m_collectdrop;
					Session[wizard.C_COLLECT_DROP_INDEX] = m_instruction.CollectDrops.Count;
				}

				switch ((eInstructionType) m_instruction.InstructionTypeId)
				{
					case eInstructionType.Load:
						lblCollection.Visible = true;
						chkAddAnotherDocket.Text += "collection.";
						lblDelivery.Visible = false;
						trSelectDocket.Visible = false;
						lblCollectDrop.Text = "Collection Details";
                        if (!m_isAmendment && m_collectDropIndex == 0)
							dteBookedDate.Text = DateTime.UtcNow.ToString("dd/MM/yy");
						break;
					case eInstructionType.Drop:
						lblCollection.Visible = false;
						chkAddAnotherDocket.Text += "delivery.";
						lblDelivery.Visible = true;
						trSelectDocket.Visible = true;

						// Stop the user from editing the docket information
						txtDocket.Enabled = false;
						txtQuantityCases.Enabled = false;
						txtPallets.Enabled = false;
						txtWeight.Enabled = false;
						lblCollectDrop.Text = "Delivery Details";
                        cboGoodsType.Enabled = false;

						// Populate the dockets
						PopulateDockets();
						break;
				}

                if (m_collectdrop != null)
                {
                    if (m_isUpdate)
                        cboGoodsType.DataSource = Facade.GoodsType.GetGoodsTypesForClientAndCollectDrop(m_job.IdentityId, m_collectdrop.CollectDropId);
                    else
                        cboGoodsType.DataSource = Facade.GoodsType.GetGoodsTypesForClient(m_job.IdentityId);
                    cboGoodsType.DataBind();

                    // Populate the collect drop information onto the form
                    txtDocket.Text = m_collectdrop.Docket;
                    txtQuantityCases.Text = (m_collectdrop.NoCases == 0 && !m_isAmendment) ? "" : m_collectdrop.NoCases.ToString();
                    txtPallets.Text = (m_collectdrop.NoPallets == 0 && !m_isAmendment) ? "" : m_collectdrop.NoPallets.ToString();
                    txtWeight.Text = (m_collectdrop.Weight == 0 && !m_isAmendment) ? "" : m_collectdrop.Weight.ToString();
                    if (m_isAmendment || m_isUpdate)
                    {
                        cboGoodsType.ClearSelection();
                        ListItem item = cboGoodsType.Items.FindByValue(m_collectdrop.GoodsTypeId.ToString());
                        if (item != null)
                            item.Selected = true;
                    }
                    txtNotes.Text = m_instruction.Note;
                }
			}	
			else
				m_collectdrop = (Entities.CollectDrop) Session[wizard.C_COLLECT_DROP];
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			cboDockets.SelectedIndexChanged += new EventHandler(cboDockets_SelectedIndexChanged);

			cfvBookedDate.ServerValidate += new ServerValidateEventHandler(cfvBookedDate_ServerValidate);
			cfvDocket.ServerValidate += new ServerValidateEventHandler(cfvDocket_ServerValidate);
			cfvQuantityCases.ServerValidate += new ServerValidateEventHandler(cfvQuantityCases_ServerValidate);
			cfvPallets.ServerValidate += new ServerValidateEventHandler(cfvPallets_ServerValidate);
			cfvWeight.ServerValidate += new ServerValidateEventHandler(cfvWeight_ServerValidate);
            this.cfvValidDateRange.ServerValidate += new ServerValidateEventHandler(cfvValidDateRange_ServerValidate);
            cboGoodsType.DataBound += new EventHandler(cboGoodsType_DataBound);

			btnRemoveDocket.Click += new EventHandler(btnRemoveDocket_Click);

			btnBack.Click += new EventHandler(btnBack_Click);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}

        

	
		#endregion

		#region Methods & Event Handlers
		
		#region Methods
		
		private void ConfigureDocketTerminology()
		{
			Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
			string docketNumberText = facReferenceData.GetDocketNumberTextForIdentityId(m_job.IdentityId);
			
			if (docketNumberText == String.Empty)
				docketNumberText = "Docket Number";

			// Details of added Dockets
			lblDocketNumber.Text = docketNumberText;
			if (m_instruction.CollectDrops != null)
			{
				if (m_instruction.CollectDrops.Count != 0)
				{
					lblAddedDockets.Text  = "Added " + docketNumberText + "s";
					lblAddedDockets.Visible = true;
				}
				else
				{
					lblAddedDockets.Visible = false;
				}
			}

			rfvDocket.ErrorMessage = "Please select a " + docketNumberText + ".";
			rfvDocket.Text = rfvDocket.Text.Replace("Docket Number.", docketNumberText + ".");

			lblSelectDocket.Text = "Select " + docketNumberText;
			rfvDockets.InitialValue = "-- Please select a " + docketNumberText + " --";
			rfvDockets.ErrorMessage = "Please enter a " + docketNumberText + ".";
			rfvDockets.Text = rfvDockets.Text.Replace("Docket Number.", docketNumberText + ".");

			chkAddAnotherDocket.Text = chkAddAnotherDocket.Text.Replace("docket number", docketNumberText.ToLower());
		}

		private ListItem DocketItem(Entities.CollectDrop collectDrop)
		{
			ListItem li = new ListItem(collectDrop.Docket, collectDrop.Docket);

			return li;
		}

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void PopulateDockets()
		{
			cboDockets.Items.Clear();
			Facade.IReferenceData facReferenceData = new Facade.ReferenceData();

			string docketNumberText = facReferenceData.GetDocketNumberTextForIdentityId(m_job.IdentityId);
			if (docketNumberText != String.Empty)
				cboDockets.Items.Add(new ListItem("-- Please select a " + docketNumberText + " --"));
			else
				cboDockets.Items.Add(new ListItem("-- Please select a Docket Number --"));

			if (m_isUpdate)
			{
				#region Display the dockets that have not been been assigned to a drop

                foreach (Entities.Instruction instruction in m_job.Instructions)
				{
					if (instruction.InstructionTypeId == (int) eInstructionType.Load)
					{
						foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
						{
							bool collectDropHasBeenDropped = false;

							foreach (Entities.Instruction dropInstruction in m_job.Instructions)
							{
								if (dropInstruction.InstructionTypeId == (int) eInstructionType.Drop)
								{
									foreach (Entities.CollectDrop dropCollectDrop in dropInstruction.CollectDrops)
									{
										if (dropCollectDrop.Docket == collectDrop.Docket)
											collectDropHasBeenDropped = true;
									}
								}
							}

							if (m_instruction.InstructionTypeId == (int) eInstructionType.Drop)
							{
								foreach (Entities.CollectDrop dropCollectDrop in m_instruction.CollectDrops)
								{
									if (dropCollectDrop.Docket == collectDrop.Docket)
										collectDropHasBeenDropped = true;
								}
							}

							if (!collectDropHasBeenDropped)
								cboDockets.Items.Add(DocketItem(collectDrop));
						}
					}
				}

				#endregion

				if (m_isAmendment)
				{
					cboDockets.ClearSelection();

					cboDockets.Items.Insert(1, DocketItem(m_collectdrop));
					cboDockets.Items[1].Selected = true;
				}
			}
			else
			{
				#region Display a list of dockets that have not been assigned to a drop

				// Check both within the m_job.Instructions collection, but also the new collection being built-up.
				Entities.InstructionCollection insCollection = new Entities.InstructionCollection();
				foreach (Entities.Instruction instruction in m_job.Instructions)
					insCollection.Add(instruction);
				insCollection.Add(m_instruction);

				foreach (Entities.Instruction instruction in insCollection)
				{
					if (instruction.InstructionTypeId == (int) eInstructionType.Load)
					{
						foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
						{
							bool collectDropHasBeenDropped = false;

							foreach (Entities.Instruction dropInstruction in insCollection)
							{
								if (dropInstruction.InstructionTypeId == (int) eInstructionType.Drop)
								{
									foreach (Entities.CollectDrop dropCollectDrop in dropInstruction.CollectDrops)
									{
										if (collectDrop.Docket == dropCollectDrop.Docket)
											collectDropHasBeenDropped = true;
                                     }
								}
							}
                            
							if (!collectDropHasBeenDropped)
								cboDockets.Items.Add(DocketItem(collectDrop));
						}
					}
				}

				#endregion

				if (m_isAmendment)
				{
					cboDockets.ClearSelection();
					// Add and select the docket
					cboDockets.Items.Insert(1, DocketItem(m_collectdrop));
					cboDockets.Items[1].Selected = true;
				}
			}

			if (m_instruction.InstructionTypeId == (int) eInstructionType.Drop && cboDockets.Items.Count == 2)
				chkAddAnotherDocket.Enabled = false;
		}

		private void cboDockets_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Find the collection docket and import it's fields
			if (m_isUpdate)
			{
				#region Find in the legs collection

                foreach (Entities.Instruction instruction in m_job.Instructions)
				{
					if (instruction.InstructionTypeId == (int) eInstructionType.Load)
					{
						foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
						{
							if (collectDrop.Docket == cboDockets.SelectedValue)
							{
								txtDocket.Text = collectDrop.Docket;
								txtQuantityCases.Text = collectDrop.NoCases.ToString();
								txtPallets.Text = collectDrop.NoPallets.ToString();
								txtWeight.Text = collectDrop.Weight.ToString();
                                cboGoodsType.ClearSelection();
                                cboGoodsType.Items.FindByValue(collectDrop.GoodsTypeId.ToString()).Selected = true;
							}
						}
					}
				}

				#endregion
			}
			else
			{
				#region Find in the instructions collection

				foreach (Entities.Instruction instruction in m_job.Instructions)
				{
					if (instruction.InstructionTypeId == (int) eInstructionType.Load)
					{
						foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
						{
							if (collectDrop.Docket == cboDockets.SelectedValue)
							{
								txtDocket.Text = collectDrop.Docket;
								txtQuantityCases.Text = collectDrop.NoCases.ToString();
								txtPallets.Text = collectDrop.NoPallets.ToString();
								txtWeight.Text = collectDrop.Weight.ToString();
                                cboGoodsType.ClearSelection();
                                cboGoodsType.Items.FindByValue(collectDrop.GoodsTypeId.ToString()).Selected = true;
                            }
						}
					}
				}

				#endregion
			}
		}

	
		#endregion
		
		#region Validation

		/// <summary>
		/// Ensure that the booked datetime entered is feasible.  For collections it must be before the first delivery, and after the last 
		/// completed collection.  For deliveries it must be after the last collection and after the last completed delivery.
		/// </summary>
		private void cfvBookedDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			DateTime booked = dteBookedDate.SelectedDate.Value;
			booked = booked.Subtract(booked.TimeOfDay);
			
			if (dteBookedTime.Text == "AnyTime")
				booked = booked.Add(new TimeSpan(23, 59, 59));
			else
				booked = booked.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);

            bool lastCollectionIsAnyTime = false;
			DateTime lastCollection = DateTime.MinValue;
			DateTime firstDelivery = DateTime.MaxValue;

            // Get the last collection and first delivery booked date
			if (m_isUpdate)
			{
				Facade.IInstructionActual facInstructionActual = new Facade.Instruction();

                foreach (Entities.Instruction instruction in m_job.Instructions)
				{
                    switch (instruction.InstructionTypeId)
					{
						case (int) eInstructionType.Load:
                            if (instruction.CollectionDateTime > lastCollection)
							{
                                lastCollection = instruction.CollectionDateTime;
                                lastCollectionIsAnyTime = instruction.IsAnyTime;
							}
							break;
						case (int) eInstructionType.Drop:
                            if (instruction.DropDateTime < firstDelivery)
                                firstDelivery = instruction.DropDateTime;
							break;
					}
				}
			}
			else
			{
				foreach (Entities.Instruction instruction in m_job.Instructions)
				{
					switch (instruction.InstructionTypeId)
					{
						case (int) eInstructionType.Load:
							if (instruction.CollectionDateTime > lastCollection)
								lastCollection = instruction.CollectionDateTime;
								lastCollectionIsAnyTime = instruction.IsAnyTime;
							break;
						case (int) eInstructionType.Drop:
							if (instruction.DropDateTime < firstDelivery)
								firstDelivery = instruction.DropDateTime;
							break;
					}
				}
			}

			if (lastCollectionIsAnyTime && m_instruction.InstructionTypeId == (int) eInstructionType.Drop)
				lastCollection = lastCollection.Subtract(lastCollection.TimeOfDay);

			switch (m_instruction.InstructionTypeId)
			{
				case (int) eInstructionType.Load:
					// The collection must occur before the first delivery.
					args.IsValid = booked <= firstDelivery;
					break;
				case (int) eInstructionType.Drop:
					// The delivery must occur after the last collection.
					args.IsValid = booked >= lastCollection;
					break;
			}
		}

		/// <summary>
		/// The docket number entered must not appear on any other collection in the job.
		/// </summary>
		private void cfvDocket_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;

			// This validation only applies for load instructions, drops do not allow the collect drop docket number to be altered.
			if (m_instruction.InstructionTypeId == (int) eInstructionType.Load)
			{
				if (m_isUpdate)
				{
					// See if the docket number can be found in any of the collections in m_job.
                    foreach (Entities.Instruction instruction in m_job.Instructions)
						if (instruction.InstructionTypeId == (int) eInstructionType.Load && instruction.InstructionID != m_instruction.InstructionID)
							foreach (Entities.CollectDrop collect in instruction.CollectDrops)
								if (collect.Docket == txtDocket.Text)
									args.IsValid = false;

					// See if the docket number can be found in the current instruction.
					for (int collectIndex = 0; collectIndex < m_instruction.CollectDrops.Count; collectIndex++)
					{
						if (collectIndex != m_collectDropIndex)
						{
							Entities.CollectDrop thisCollect = m_instruction.CollectDrops[collectIndex];

							if (thisCollect.Docket == txtDocket.Text)
								args.IsValid = false;
						}
					}
				}
				else
				{
					int currentInstructionIndex = (int) Session[wizard.C_INSTRUCTION_INDEX];

					// See if the docket number can be found in any of the collections in m_job.Instructions
					for (int instructionIndex = 0; instructionIndex < m_job.Instructions.Count; instructionIndex++)
					{
						Entities.Instruction instruction = m_job.Instructions[instructionIndex];

						if (instruction.InstructionTypeId == (int) eInstructionType.Load && instructionIndex != currentInstructionIndex)
						{
							for (int collectIndex = 0; collectIndex < instruction.CollectDrops.Count; collectIndex++)
							{
								Entities.CollectDrop thisCollect = instruction.CollectDrops[collectIndex];

								if (thisCollect.Docket == txtDocket.Text)
								{
									if (instructionIndex != currentInstructionIndex || collectIndex != m_collectDropIndex)
										args.IsValid = false;
								}
							}
						}
					}

					// See if the docket can be found in the current instruction
					for (int collectIndex = 0; collectIndex < m_instruction.CollectDrops.Count; collectIndex++)
					{
						if (collectIndex != m_collectDropIndex)
						{
							Entities.CollectDrop thisCollect = m_instruction.CollectDrops[collectIndex];

							if (thisCollect.Docket == txtDocket.Text)
								args.IsValid = false;
						}
					}
				}
			}
		}

		private void cfvPallets_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
		}
        void cfvValidDateRange_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime val = DateTime.Parse(args.Value);
            DateTime minDate = DateTime.Today.AddDays(-1);
            DateTime maxDate = DateTime.Today.AddDays(3);
            if (this.ViewState["_ignorecfvValidDateRange"] != null && (((DateTime)this.ViewState["_ignorecfvValidDateRange_Value"] == val) &&  (bool)this.ViewState["_ignorecfvValidDateRange"]))
                args.IsValid = true;
            else
            {
                args.IsValid = (val >= minDate && val <= maxDate);
                this.ViewState["_ignorecfvValidDateRange_Value"] = val;
                this.ViewState["_ignorecfvValidDateRange"] = true;
            }
        }

		private void cfvQuantityCases_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
		}

		private void cfvWeight_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
		}
		
		#endregion

		#region Button Event Handlers

		private void btnBack_Click(object sender, EventArgs e)
		{
			if (m_collectDropIndex == 0)
				GoToStep("P");
			else
			{
				Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex - 1];
				Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex - 1;
				GoToStep("PD");
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				// Populate the collect drop summary information
				DateTime bookedDateTime = dteBookedDate.SelectedDate.Value;
				if (dteBookedTime.Text == "AnyTime")
				{
					bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
					bookedDateTime = bookedDateTime.Add(new TimeSpan(23, 59, 59));
					m_instruction.BookedDateTime = bookedDateTime;
					m_instruction.IsAnyTime = true;
				}
				else
				{
					bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
					bookedDateTime = bookedDateTime.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);
					m_instruction.BookedDateTime = bookedDateTime;
					m_instruction.IsAnyTime = false;
				}
				m_instruction.Note = txtNotes.Text;

				// Populate the collect drop information
				string oldDocket = m_collectdrop.Docket;
				m_collectdrop.Docket = txtDocket.Text;
				
				m_collectdrop.NoCases = 0;
				m_collectdrop.NoPallets = 0;
				m_collectdrop.Weight = 0;
                int goodsTypeId = 0;
                if (Int32.TryParse(cboGoodsType.SelectedValue, out goodsTypeId))
                    m_collectdrop.GoodsTypeId = goodsTypeId;
                else
                    m_collectdrop.GoodsTypeId = 0;
                m_collectdrop.GoodsTypeDescription = cboGoodsType.SelectedItem.Text;
				
				try { m_collectdrop.NoCases = Convert.ToInt32(txtQuantityCases.Text); } 
				catch {}
				try { m_collectdrop.NoPallets = Convert.ToInt32(txtPallets.Text); } 
				catch {}
				try { m_collectdrop.Weight = Convert.ToDecimal(txtWeight.Text); } 
				catch {}

				if (m_isUpdate)
				{
					if (m_collectdrop.CollectDropId > 0)
					{
						// Find the collect drop information and replace it
						for (int i = 0; i < m_instruction.CollectDrops.Count; i++)
						{
							if (m_instruction.CollectDrops[i].CollectDropId == m_collectdrop.CollectDropId)
							{
								m_instruction.CollectDrops.RemoveAt(i);
								m_instruction.CollectDrops.Insert(i, m_collectdrop);

								// There may be a delivery that needs updating
                                foreach (Entities.Instruction instruction in m_job.Instructions)
								{
									if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
									{
										foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
										{
											if (collectDrop.Docket == oldDocket)
											{
												// Update this collect drop information also
												collectDrop.Docket = m_collectdrop.Docket;
												collectDrop.ClientsCustomerReference = m_collectdrop.ClientsCustomerReference;
												collectDrop.NoCases = m_collectdrop.NoCases;
												collectDrop.NoPallets = m_collectdrop.NoPallets;
												collectDrop.Weight = m_collectdrop.Weight;
                                                collectDrop.GoodsTypeId = m_collectdrop.GoodsTypeId;
                                                collectDrop.GoodsTypeDescription = m_collectdrop.GoodsTypeDescription;
											}
										}
									}
								}
							}
						}
					}
					else
					{
						bool found = false;

						foreach (Entities.CollectDrop cd in m_instruction.CollectDrops)
							if (cd.Docket == m_collectdrop.Docket)
								found = true;

						if (!found)
							m_instruction.CollectDrops.Add(m_collectdrop);
					}
				}
				else
				{
					if (m_isAmendment)
					{
						if (m_collectDropIndex == m_instruction.CollectDrops.Count)
						{
							// Add the collect drop to the collection
							m_instruction.CollectDrops.Add(m_collectdrop);
						}
						else
						{
							// Update the specified collect drop
							m_instruction.CollectDrops.RemoveAt(m_collectDropIndex);
							m_instruction.CollectDrops.Insert(m_collectDropIndex, m_collectdrop);

							// There may be a delivery that needs updating
							foreach (Entities.Instruction instruction in m_job.Instructions)
							{
								if (instruction.InstructionTypeId == (int) eInstructionType.Drop)
								{
									foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
									{
										if (collectDrop.Docket == oldDocket)
										{
											// Update this collect drop information also
											collectDrop.Docket = m_collectdrop.Docket;
											collectDrop.ClientsCustomerReference = m_collectdrop.ClientsCustomerReference;
											collectDrop.NoCases = m_collectdrop.NoCases;
											collectDrop.NoPallets = m_collectdrop.NoPallets;
											collectDrop.Weight = m_collectdrop.Weight;
                                            collectDrop.GoodsTypeId = m_collectdrop.GoodsTypeId;
                                            collectDrop.GoodsTypeDescription = m_collectdrop.GoodsTypeDescription;
										}
									}
								}
							}
						}
					}
					else
					{
						// Add the collect drop to the collection
						m_instruction.CollectDrops.Add(m_collectdrop);
					}
				}

				// Configure session variables
				Session[wizard.C_INSTRUCTION] = m_instruction;
				Session[wizard.C_COLLECT_DROP] = null;
				Session[wizard.C_COLLECT_DROP_INDEX] = null;

				if (m_isUpdate)
				{
					m_collectDropIndex++;

					if (m_collectDropIndex < m_instruction.CollectDrops.Count)
					{
						Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];
						Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
							
						GoToStep("PD");
					}
					else
					{
						if (chkAddAnotherDocket.Checked)
						{
							Session[wizard.C_COLLECT_DROP_INDEX] = m_instruction.CollectDrops.Count;
							GoToStep("PD");
						}
						else
							GoToStep("PC");
					}
				}
				else
				{
					if (m_isAmendment)
					{
						m_collectDropIndex++;

						if (m_collectDropIndex < m_instruction.CollectDrops.Count)
						{
							Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];
							Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
							
							GoToStep("PD");
						}
						else
						{
							if (chkAddAnotherDocket.Checked)
							{
								Session[wizard.C_COLLECT_DROP_INDEX] = m_instruction.CollectDrops.Count;
								GoToStep("PD");
							}
							else
								GoToStep("PC");
						}
					}
					else
					{
						// Go to the next step
						if (chkAddAnotherDocket.Checked)
						{
							Session[wizard.C_COLLECT_DROP_INDEX] = m_instruction.CollectDrops.Count;
							GoToStep("PD");
						}
						else
							GoToStep("PC");
					}
				}
			}
		}

		private void btnRemoveDocket_Click(object sender, EventArgs e)
		{
            if (m_collectDropIndex == m_instruction.CollectDrops.Count)
            {
                // This was a new docket anyway, so just go back to the previous one.
                if (m_collectDropIndex > 0)
                {
                    Session[wizard.C_INSTRUCTION] = m_instruction;
                    m_collectDropIndex--;
                    Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
                    Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];
                }
            }
            else
            {
                // Remove the docket from the instruction.
                m_instruction.CollectDrops.RemoveAt(m_collectDropIndex);
                Session[wizard.C_INSTRUCTION] = m_instruction;

                if (m_collectDropIndex > 0)
                    m_collectDropIndex--;

                Session[wizard.C_COLLECT_DROP_INDEX] = m_collectDropIndex;
                Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[m_collectDropIndex];
            }

			GoToStep("PD");
		}

		#endregion

        #region Drop Down Event Handlers

        void cboGoodsType_DataBound(object sender, EventArgs e)
        {
            // If this is a new docket, select the default goods type for this client.
            if (!m_isAmendment && !m_isUpdate)
            {
                DataSet ds = (DataSet)cboGoodsType.DataSource;
                cboGoodsType.ClearSelection();

                foreach (DataRow row in ds.Tables[0].Rows)
                    if ((bool)row["IsDefault"])
                        cboGoodsType.Items.FindByValue(((int)row["GoodsTypeId"]).ToString()).Selected = true;
            }
        }

        #endregion

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
