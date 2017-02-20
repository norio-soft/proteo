namespace Orchestrator.WebUI.Job.Wizard.UserControls
{		
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for AddPoint.
	/// </summary>
    public partial class AddPoint : System.Web.UI.UserControl, IDefaultButton
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
		private bool			m_isAmendment = false;

		private Entities.Instruction	m_instruction;
		private int						m_instructionIndex;

		protected string		m_company;
		protected string		m_town;
		

		protected string		m_description;

		#endregion

		#region Page Load/Init/Error
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
				m_isUpdate = true;

			// Retrieve the job from the session variable
			m_job = (Entities.Job) Session[wizard.C_JOB];

			if (Session[wizard.C_INSTRUCTION_INDEX] != null)
			{
				m_instructionIndex = (int) Session[wizard.C_INSTRUCTION_INDEX];

				if (!m_isUpdate && m_instructionIndex != m_job.Instructions.Count)
					m_isAmendment = true;
			}

            if (Session[wizard.C_INSTRUCTION] != null)
                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

            if (!IsPostBack)
            {
                // Set the company
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                m_company = facOrganisation.GetForIdentityId((int)Session[wizard.C_POINT_FOR]).OrganisationName;

                // Set the town
                Facade.IPostTown facPostTown = new Facade.Point();
                m_town = facPostTown.GetPostTownForTownId((int)Session[wizard.C_TOWN_ID]).TownName;

                // Set the description
                m_description = Session[wizard.C_POINT_NAME].ToString();

                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

                btnNext.Attributes.Add("onClick", "javascript:HidePage();"); 
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);
            }
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			btnBack.Click += new EventHandler(btnBack_Click);
			btnNext.Click += new EventHandler(btnNext_Click);
			btnCancel.Click += new EventHandler(btnCancel_Click);
		}

		
		#endregion 

		#region Event Handlers & Events

		private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
			
			if (m_isUpdate)
				url += "&jobId=" + m_jobId.ToString();

			Response.Redirect(url);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			// Clear the session variables used to help add the new point
			Session[wizard.C_POINT_TYPE] = null;
			Session[wizard.C_POINT_FOR] = null;
			Session[wizard.C_POINT_NAME] = null;
			Session[wizard.C_TOWN_ID] = null;

			GoToStep("P");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				#region Create the new point

				Entities.Point newPoint = new Entities.Point();

				Facade.IPostTown facPostTown = new Facade.Point();
				Entities.PostTown town = facPostTown.GetPostTownForTownId((int) Session[wizard.C_TOWN_ID]);

				// Set the point owner and description
				newPoint.IdentityId = (int) Session[wizard.C_POINT_FOR];
				newPoint.Description = Session[wizard.C_POINT_NAME].ToString();
				
				// Get the point type
				switch ((ePointType) Session[wizard.C_POINT_TYPE])
				{
					case ePointType.Collect:
						newPoint.Collect = true;
						break;
					case ePointType.Deliver:
						newPoint.Deliver = true;
						break;
					case ePointType.Any:
						newPoint.Collect = true;
						newPoint.Deliver = true;
						break;
				}

				// set the address
				Entities.Address address = new Entities.Address();
				address.AddressLine1 = txtAddressLine1.Text;
				address.AddressLine2 = txtAddressLine2.Text;
				address.AddressLine3 = txtAddressLine3.Text;
				address.AddressType = eAddressType.Point;
				address.County = txtCounty.Text;
				address.IdentityId = newPoint.IdentityId;
				address.Latitude = Decimal.Parse(txtLatitude.Text);
				address.Longitude = Decimal.Parse(txtLongitude.Text);
                address.PostCode = txtPostCode.Text;
				address.PostTown = txtPostTown.Text;
				if (address.TrafficArea == null)
					address.TrafficArea = new Orchestrator.Entities.TrafficArea();

				address.TrafficArea.TrafficAreaId = Convert.ToInt32(hidTrafficArea.Value);
				newPoint.Address = address;
				newPoint.Longitude = address.Longitude;
				newPoint.Latitude = address.Latitude;

                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Orchestrator.Entities.Organisation organisation = facOrganisation.GetForIdentityId(newPoint.IdentityId);

                // set the radius if the address was changed by addressLookup
                // if the org has a default, use it, if not, use the system default.
                if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                    if (organisation.Defaults[0].DefaultGeofenceRadius == null)
                        newPoint.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
                    else
                        newPoint.Radius = organisation.Defaults[0].DefaultGeofenceRadius;

				newPoint.PostTown = town;
                newPoint.PointNotes = txtPointNotes.Text;
				
				string userId = ((Entities.CustomPrincipal) Page.User).UserName;

				// Create the new point
				Facade.IPoint facPoint = new Facade.Point();
				Entities.FacadeResult result = facPoint.Create(newPoint, userId);

				int pointId = facPoint.Create(newPoint, userId).ObjectId;

				#endregion

				if (pointId == 0)
				{
					// This customer already has a point with this name
					// Try to locate the point
					Entities.PointCollection points = facPoint.GetClientPointsForName(newPoint.IdentityId, newPoint.Description);
					if (points.Count == 1)
					{
						// The point id has been found!
						pointId = points[0].PointId;
					}
					else
					{
						// Clear the session variables used to help add the new point
						Session[wizard.C_POINT_TYPE] = null;
						Session[wizard.C_POINT_FOR] = null;
						Session[wizard.C_POINT_NAME] = null;
						Session[wizard.C_TOWN_ID] = null;

						GoToStep("P");
					}
				}

				if (pointId > 0)
				{
					// Reload the point to ensure we have all the ancillary entities
					Entities.Point createdPoint = facPoint.GetPointForPointId(pointId);

					if (m_isUpdate)
					{
						// Add the collect drop point information
						if (m_instruction == null)
						{
							m_instruction = new Entities.Instruction();

							m_instruction.JobId = m_jobId;
							switch ((ePointType) (int) Session[wizard.C_POINT_TYPE])
							{
								case ePointType.Collect:
									m_instruction.InstructionTypeId = (int) eInstructionType.Load;
									break;
								case ePointType.Deliver:
									m_instruction.InstructionTypeId = (int) eInstructionType.Drop;
									break;
							}
						}

						m_instruction.Point = createdPoint;
						m_instruction.PointID = createdPoint.PointId;
						m_instruction.InstructionID = m_instruction.InstructionID;

						if (m_instruction.InstructionTypeId == (int) eInstructionType.Drop)
							m_instruction.ClientsCustomerIdentityID = createdPoint.IdentityId;

                        // Cause the first docket to be displayed.
                        if (m_instruction.CollectDrops.Count > 0)
                        {
                            Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[0];
                            Session[wizard.C_COLLECT_DROP_INDEX] = 0;
                        } 
                        
                        Session[wizard.C_INSTRUCTION] = m_instruction;
					}
					else
					{
						if (m_isAmendment)
						{
							if (pointId != m_instruction.PointID)
							{
								m_instruction.Point = createdPoint;
								m_instruction.PointID = createdPoint.PointId;
							}

							// Cause the first docket to be displayed.
							if (m_instruction.CollectDrops.Count > 0)
							{
								Session[wizard.C_COLLECT_DROP] = m_instruction.CollectDrops[0];
								Session[wizard.C_COLLECT_DROP_INDEX] = 0;
							}

							Session[wizard.C_INSTRUCTION] = m_instruction;
						}
						else
						{
							// Add the collect drop point information
							if (m_instruction == null)
							{
								m_instruction = new Entities.Instruction();

								m_instruction.JobId = m_jobId;
								switch ((ePointType) (int) Session[wizard.C_POINT_TYPE])
								{
									case ePointType.Collect:
										m_instruction.InstructionTypeId = (int) eInstructionType.Load;
										break;
									case ePointType.Deliver:
										m_instruction.InstructionTypeId = (int) eInstructionType.Drop;
										break;
								}
							}

							m_instruction.Point = createdPoint;
							m_instruction.PointID = createdPoint.PointId;
							m_instruction.InstructionID = m_instruction.InstructionID;

							if (m_instruction.InstructionTypeId == (int) eInstructionType.Drop)
								m_instruction.ClientsCustomerIdentityID = createdPoint.IdentityId;

							Session[wizard.C_INSTRUCTION] = m_instruction;
						}
					}

					GoToStep("PD");
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
