using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class CommunicateThis : Orchestrator.Base.BasePage
    {

        #region Protected Fields

        protected int _driverId = 0;
        protected int _subContractorId = 0;
        protected int _jobId = 0;
        protected int _instructionID = 0;

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["DR"]))
                _driverId = int.Parse(Request.QueryString["DR"]);

            if (!string.IsNullOrEmpty(Request.QueryString["SubbyId"]))
                _subContractorId = int.Parse(Request.QueryString["SubbyId"]);

            _jobId = int.Parse(Request.QueryString["jobId"]);
            _instructionID = int.Parse(Request.QueryString["iID"]);

            if (!IsPostBack)
            {
                if (_subContractorId > 0)
                    LoadCommunicationForSubContractor();
                else
                    LoadCommunication();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCommunicate.Click += new EventHandler(btnCommunicate_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.cboControlArea.SelectedIndexChanged += new EventHandler(cboControlArea_SelectedIndexChanged);
        }

        void cboControlArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTrafficAreas(Convert.ToInt32(cboControlArea.SelectedValue));
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "refresh";
            this.Close();
        }

        void btnCommunicate_Click(object sender, EventArgs e)
        {
            UpdateValidators();
            this.Validate();

            if (Page.IsValid)
            {
                if (btnCommunicate.Text == "Close")
                {
                    this.Close();
                }
                else
                {
                    Communicate();
                    pnlForm.Visible = false;
                    pnlConfirmation.Visible = true;
                    btnCommunicate.Visible = false;
                    btnCancel.Text = "Close";
                    this.ReturnValue = "refresh";
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateValidators()
        {
            bool isRFVNumber = true, isRFVSMSText = true;

            bool.TryParse(hdnValidateNumber.Value, out isRFVNumber);
            bool.TryParse(hdnValidateSMSText.Value, out isRFVSMSText);

            rfvNumber.Enabled = isRFVNumber;
            rfvSMSText.Enabled = isRFVSMSText;
        }

        private void LoadCommunication()
        {
            Facade.IInstruction facInstrucion = new Facade.Instruction();
            var jobInstructions = facInstrucion.GetForJobId(_jobId);
            var instruction = jobInstructions.Single(i => i.InstructionID == _instructionID);
            
            // Get the driver.
            Facade.IDriver facDriver = new Facade.Resource();
            Entities.Driver driver = facDriver.GetDriverForResourceId(_driverId);

            var communicationTypes = Utilities.GetEnumPairs<eDriverCommunicationType>();
            
            // We don't offer the option to communicate by Tough Touch if the driver doesn't have a passcode or the vehicle doesn't have a Tough Touch installed.
            if (string.IsNullOrWhiteSpace(driver.Passcode) || instruction.Vehicle == null || !instruction.Vehicle.IsTTInstalled)
                communicationTypes.Remove((int)eDriverCommunicationType.ToughTouch);

            cboCommunicationType.DataSource = communicationTypes;
            cboCommunicationType.DataBind();
            cboCommunicationType.Items.Insert(0, new ListItem("-- Select a communication type --", ""));
            cboCommunicationType.Attributes.Add("onchange", "cboCommunicationTypeIndex_Changed();");

            cboCommunicationStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eDriverCommunicationStatus)));
            cboCommunicationStatus.DataBind();

            RadioButton rb = null;

            Entities.ContactCollection contacts = new Entities.ContactCollection();

            if (driver != null && driver.Individual.Contacts != null)
                contacts = driver.Individual.Contacts;

            // Get the vehicle the driver is currently assigned to.
            Facade.IJourney facJourney = new Facade.Journey();
            Entities.Vehicle currentVehicle = facJourney.GetCurrentVehicleForDriver(_driverId);

            if (currentVehicle != null)
                contacts.Add(new Entities.Contact(eContactType.MobilePhone, currentVehicle.CabPhoneNumber));

            Entities.ContactCollection cs = new Orchestrator.Entities.ContactCollection();
            rb = new RadioButton();

            bool numberSelected = false;
            foreach (Entities.Contact contact in contacts)
            {
                if (contact.ContactDetail.Length > 0)
                {
                    rb = new RadioButton();
                    rb.GroupName = "rblNumbers";
                    rb.Text = contact.ContactDetail;
                    rb.Attributes.Add("onclick", "setNumberUsed('" + contact.ContactDetail + "');");

                    if (contact.ContactType == eContactType.MobilePhone || contact.ContactType == eContactType.PersonalMobile
                        || contact.ContactType == eContactType.Telephone)
                    {
                        if (!numberSelected)
                        {
                            rb.Checked = true;
                            txtNumber.Text = contact.ContactDetail;
                            numberSelected = true;
                        }
                    }

                    plcNumbers.Controls.Add(rb);
                    plcNumbers.Controls.Add(new LiteralControl("<br/>"));
                }
            }

            rb = new RadioButton();
            rb.Text = "Other";
            rb.GroupName = "rblNumbers";
            rb.Attributes.Add("onclick", "enableOtherTextBox();");
            plcNumbers.Controls.Add(rb);

            bool isInvolvedInLoad = false;

            foreach (Entities.Instruction i in jobInstructions)
            {
                if (i.Driver != null && i.Driver.ResourceId == _driverId)
                {
                    if ((eInstructionType)i.InstructionTypeId == eInstructionType.Load)
                        isInvolvedInLoad = true;
                }
            }

            trLoadOrder.Visible = isInvolvedInLoad;

            if (isInvolvedInLoad)
            {
                var loadComments = BuildLoadComments(_driverId, jobInstructions);
                lblLoadOrder.Text = loadComments;
                trLoadOrder.Visible = !string.IsNullOrWhiteSpace(loadComments);
            }

            var legPlan = new Facade.Instruction().GetLegPlan(jobInstructions, false);

            txtComments.Text = GetComments(legPlan, eDriverCommunicationType.Phone);
            txtSMSText.Text = GetComments(legPlan, eDriverCommunicationType.Text);

            var defaultCommunicationTypeID = driver.DefaultCommunicationTypeID == 0 ? (int)eDriverCommunicationType.Phone : driver.DefaultCommunicationTypeID;
            //lookup the communication type in the drop down, dont select if it doesnt exist - this should only happen if the driver whos default method of communitication
            // is tough touch but has now moved into a vechicial without one being installed
            if(cboCommunicationType.Items.FindByValue(defaultCommunicationTypeID.ToString())!=null)
                cboCommunicationType.Items.FindByValue(defaultCommunicationTypeID.ToString()).Selected = true;

            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataBind();

            for (int instructionIndex = jobInstructions.Count - 1; instructionIndex >= 0; instructionIndex--)
            {
                if (jobInstructions[instructionIndex].Driver != null && jobInstructions[instructionIndex].Driver.ResourceId == _driverId)
                {
                    hidLastInstructionInvolvedWith.Value = jobInstructions[instructionIndex].InstructionOrder.ToString();

                    cboControlArea.ClearSelection();
                    cboControlArea.Items.FindByValue(jobInstructions[instructionIndex].Point.Address.TrafficArea.ControlAreaId.ToString()).Selected = true;
                    BindTrafficAreas(Convert.ToInt32(cboControlArea.SelectedValue), jobInstructions[instructionIndex].Point.Address.TrafficArea.TrafficAreaId);
                    break;
                }
            }

            ShowPCVS();
        }

        private void LoadCommunicationForSubContractor()
        {
            // We don't offer the option to communicate by Tough Touch to subcontractors.
            var communicationTypes = Utilities.GetEnumPairs<eDriverCommunicationType>();
            communicationTypes.Remove((int)eDriverCommunicationType.ToughTouch);

            cboCommunicationType.DataSource = communicationTypes;
            cboCommunicationType.DataBind();
            cboCommunicationType.Items.Insert(0, new ListItem("-- Select a communication type --", ""));
            cboCommunicationType.Attributes.Add("onchange", "cboCommunicationTypeIndex_Changed();");

            cboCommunicationStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eDriverCommunicationStatus)));
            cboCommunicationStatus.DataBind();

            RadioButton rb = new RadioButton();
            rb.Text = "Other";
            rb.GroupName = "rblNumbers";
            rb.Checked = true;
            txtNumber.Enabled = true;

            plcNumbers.Controls.Add(rb);

            //Give resources is not relavant for subbies 
            chkGiveResources.Checked = false;
            chkGiveResources.Visible = false;
            cboControlArea.Visible = false;
            cboTrafficArea.Visible = false;
        }

        private void ShowPCVS()
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job _job = facJob.GetJob(_jobId);
            if (_job.PCVs.Count > 0)
                pnlPCVS.Visible = true;
            else
                pnlPCVS.Visible = false;
        }

        private void BindTrafficAreas(int controlAreaId)
        {
            cboControlArea.ClearSelection();
            cboControlArea.Items.FindByValue(controlAreaId.ToString()).Selected = true;

            Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
            cboTrafficArea.DataSource = facTrafficArea.GetForControlAreaId(controlAreaId);
            cboTrafficArea.DataBind();

            cboTrafficArea.ClearSelection();
        }

        private void BindTrafficAreas(int controlAreaId, int trafficAreaId)
        {
            BindTrafficAreas(controlAreaId);

            cboTrafficArea.Items.FindByValue(trafficAreaId.ToString()).Selected = true;
        }

        protected void Communicate()
        {
            if (Page.IsValid)
            {
                string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                Entities.DriverCommunication communication = new Entities.DriverCommunication();
                communication.NumberUsed = txtNumber.Text;
                communication.Comments = txtComments.Text;
                communication.DriverCommunicationStatus = (eDriverCommunicationStatus)Enum.Parse(typeof(eDriverCommunicationStatus), cboCommunicationStatus.SelectedValue.Replace(" ", ""));

                communication.DriverCommunicationType = (eDriverCommunicationType)int.Parse(cboCommunicationType.SelectedValue);

                if (communication.DriverCommunicationType == eDriverCommunicationType.Text)
                    communication.Text = txtSMSText.Text;

                if (_subContractorId > 0)
                {
                    Facade.IJobSubContractor facSubContractor = new Facade.Job();
                    communication.DriverCommunicationId = facSubContractor.CreateCommunication(_jobId, _instructionID, _subContractorId, communication, userId);
                }
                else
                {
                    Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();

                    if (chkGiveResources.Checked)
                        communication.DriverCommunicationId = facDriverCommunication.Create(_jobId, _driverId, communication, userId, _instructionID, Convert.ToInt32(hidLastInstructionInvolvedWith.Value), Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue));
                    else
                        communication.DriverCommunicationId = facDriverCommunication.Create(_jobId, _driverId, communication, userId, _instructionID);

                }

                if (communication.DriverCommunicationId > 0)
                {
                    // The communication was succesfully stored.
                    lblConfirmation.Visible = true;
                    lblConfirmation.Text = "The communication has been stored.";

                    if (communication.DriverCommunicationType == eDriverCommunicationType.Text)
                    {
                        // Send the text message.
                        eTextAnywhereSendSMS sendSMSStatus = Utilities.SendSMS("dc" + communication.DriverCommunicationId.ToString(), "dc" + communication.DriverCommunicationId.ToString(), eTextAnywhereOriginatorType.SendDescriptiveText, communication.NumberUsed, communication.Text, userId);

                        if (sendSMSStatus != eTextAnywhereSendSMS.SMSSent)
                        {
                            // Display an error to the user
                            lblConfirmation.Text += "  But the Text message was not sent, the error code was " + Enum.GetName(typeof(eTextAnywhereSendSMS), sendSMSStatus);
                            lblConfirmation.Visible = true;
                        }
                    }
                }
                else
                {
                    // It was not possible to create the communication.
                    lblConfirmation.Visible = true;
                    lblConfirmation.Text = "The communication has not been stored.";
                }
            }
        }

        private string GetComments(Entities.LegPlan legPlan, eDriverCommunicationType communicationType)
        {
            bool doneFirstLeg = false;
            int lastLorry = 0;
            int lastTrailer = 0;

            StringBuilder sb = new StringBuilder();

            foreach (Entities.LegView leg in legPlan.Legs())
            {
                if (leg.Driver != null && leg.Driver.ResourceId == _driverId)
                {
                    if (!doneFirstLeg)
                    {
                        BuildStartComments(communicationType, sb, leg);
                        doneFirstLeg = true;
                    }

                    if (leg.Vehicle != null && leg.Vehicle.ResourceId != lastLorry)
                    {
                        BuildVehicleComments(communicationType, sb, leg.Vehicle);
                        lastLorry = leg.Vehicle.ResourceId;
                    }

                    if (leg.Trailer != null && leg.Trailer.ResourceId != lastTrailer)
                    {
                        BuildTrailerComments(communicationType, sb, leg.Trailer);
                        lastTrailer = leg.Trailer.ResourceId;
                    }

                    // We should display information about this leg point.
                    BuildLegPointComments(communicationType, sb, leg.EndLegPoint);

                    // Display any instruction information.
                    if ((leg.StartLegPoint != null && leg.EndLegPoint != null) && leg.StartLegPoint.Instruction != null)
                    {
                        BuildInstructionTypeComments(sb, (eInstructionType)leg.StartLegPoint.Instruction.InstructionTypeId);

                        // Display the timing information.
                        sb.Append(" " + leg.StartLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"));
                        sb.Append(" " + leg.EndLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"));
                        sb.Append(". ");
                    }
                    else
                    {
                        // Display the timing information.
                        sb.Append(" " + leg.EndLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"));
                        sb.Append(". ");
                    }
                }
            }

            return sb.ToString();
        }

        private string BuildLoadComments(int driverId, Entities.InstructionCollection instructionCollection)
        {
            StringBuilder loadComments = new StringBuilder();
            ArrayList collectedDockets = new ArrayList();

            // Build the list of dockets being collected.
            foreach (Entities.Instruction instruction in instructionCollection)
                if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                    foreach (Entities.CollectDrop collection in instruction.CollectDrops)
                        collectedDockets.Add(collection.Docket);

            Entities.Instruction instruc = null;

            // Display the dockets in the reverse order they are being delivered.
            for (int instructionIndex = instructionCollection.Count - 1; instructionIndex >= 0; instructionIndex--)
            {
                instruc = instructionCollection[instructionIndex];

                if (instruc.InstructionTypeId == (int)eInstructionType.Drop)
                {
                    foreach (Entities.CollectDrop delivery in instruc.CollectDrops)
                    {
                        if (collectedDockets.Contains(delivery.Docket))
                        {
                            loadComments.Append(string.Format("{0} ({1})", delivery.Docket, instruc.Point.PostTown.TownName));
                            loadComments.Append("<br>");
                        }
                    }
                }
            }

            return loadComments.ToString();
        }

        private void BuildStartComments(eDriverCommunicationType communicationType, StringBuilder sbComments, Entities.LegView leg)
        {
            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    sbComments.Append("START TIME: ");
                    break;
                case eDriverCommunicationType.Text:
                    sbComments.Append("ST: ");
                    break;
            }
            sbComments.Append(leg.StartLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"));
            sbComments.Append(". ");

            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    sbComments.Append("START PLACE: ");
                    break;
                case eDriverCommunicationType.Text:
                    sbComments.Append("SP: ");
                    break;
            }

            sbComments.Append(string.Format("{0}.", leg.StartLegPoint.Point.PostTown.TownName));
        }

        private int BuildVehicleComments(eDriverCommunicationType communicationType, StringBuilder sbComments, Entities.Vehicle vehicle)
        {
            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    sbComments.Append("LORRY: ");
                    break;
                case eDriverCommunicationType.Text:
                    sbComments.Append("V: ");
                    break;
            }
            sbComments.Append(vehicle.RegNo);
            sbComments.Append(". ");

            int lastLorry = vehicle.ResourceId;

            return lastLorry;
        }

        private void BuildTrailerComments(eDriverCommunicationType communicationType, StringBuilder sbComments, Entities.Trailer trailer)
        {
            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    sbComments.Append("TRAILER: ");
                    break;
                case eDriverCommunicationType.Text:
                    sbComments.Append("T: ");
                    break;
            }
            sbComments.Append(trailer.TrailerRef);
            sbComments.Append(". ");
        }

        private void BuildLegPointComments(eDriverCommunicationType communicationType, StringBuilder sbComments, Entities.LegPoint legPoint)
        {
            // We should display information about this leg point.
            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    sbComments.Append("STOP: ");
                    break;
                case eDriverCommunicationType.Text:
                    sbComments.Append("S: ");
                    break;
            }

            // Display the point information.
            sbComments.Append(legPoint.Point.OrganisationName);
            sbComments.Append(", ");
            sbComments.Append(legPoint.Point.Description);
            sbComments.Append(", ");
            sbComments.Append(legPoint.Point.PostTown.TownName);
        }

        private void BuildInstructionTypeComments(StringBuilder sbComments, eInstructionType instructionType)
        {
            switch (instructionType)
            {
                case eInstructionType.Load:
                    sbComments.Append(" (COLLECT) ");
                    break;
                case eInstructionType.Drop:
                    sbComments.Append(" (DROP) ");
                    break;
                case eInstructionType.PickupPallets:
                    sbComments.Append(" (PICKUP PALLETS) ");
                    break;
                case eInstructionType.DeHirePallets:
                    sbComments.Append(" (DEHIRE PALLETS) ");
                    break;
                case eInstructionType.LeavePallets:
                    sbComments.Append(" (LEAVE PALLETS) ");
                    break;
            }
        }

        #endregion
    }

}