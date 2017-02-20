using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using P1TP.Components.Web.UI;
using ComponentArt.Web.UI;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    /// <summary>
    /// Summary description for driverCommunications.
    /// </summary>
    public partial class driverCommunications : Orchestrator.Base.BasePage
    {
        #region Constants

        private const string C_JOB_VS = "C_JOB_VS";

        #endregion

        #region Page Variables

        private int m_jobId = 0;
        protected Entities.Job m_job;

        #endregion

        #region Form Elements

        protected RadioButtonList rblNumbers;

        #endregion

        #region Web Form Designer generated code

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            odsCommunications.Selecting += new ObjectDataSourceSelectingEventHandler(odsCommunications_Selecting);
            odsCommunications.Selected += new ObjectDataSourceStatusEventHandler(odsCommunications_Selected);

            gvCommunications.RowDataBound += new GridViewRowEventHandler(gvCommunications_RowDataBound);

            dlgCommunicateThis.DialogCallBack += new EventHandler(dlgCommunicateThis_DialogCallBack);
        }

        #endregion

        #region Page/Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.Communicate);
            bool canCommunicate = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.Communicate);

            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (!IsPostBack)
            {
                LoadJob();
                LoadUnCommunicatedLegs();
            }
            else
                m_job = (Entities.Job)ViewState[C_JOB_VS];

            lblConfirmation.Visible = false;
        }

        #endregion

        #region Comments Helper Methods
        
        private void BuildStartComments(eDriverCommunicationType communicationType, StringBuilder sbComments, Entities.Instruction instruction)
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
            sbComments.Append(instruction.PlannedDepartureDateTime.ToString("dd/MM/yy HH:mm"));
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
            sbComments.Append(string.Format("{0}.", instruction.Point.PostTown.TownName));
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

        #region Delivery Comments


        #endregion

        private void LoadJob()
        {
            Facade.IJob facJob = new Facade.Job();
            m_job = facJob.GetJob(m_jobId);

            if (m_job != null)
            {
                if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);

                Facade.IPCV facPCV = new Facade.PCV();
                m_job.PCVs = facPCV.GetForJobId(m_jobId);

                if (m_job.JobState == eJobState.Booked)
                    Response.Redirect("../jobManagement.aspx?jobId=" + m_jobId + "&csid=" + this.CookieSessionID);

                // Load the various parts of the job.
                Facade.IInstruction facInstruction = new Facade.Instruction();
                m_job.Instructions = facInstruction.GetForJobId(m_jobId);

                PopulateJobInformation();

            }

            ViewState[C_JOB_VS] = m_job;
        }

        private void PopulateJobInformation()
        {
            // Populate the Job fieldset.
            lblJobId.Text = m_jobId.ToString();
            ucJobStateIndicator.JobState = m_job.JobState;
            lblJobType.Text = Utilities.UnCamelCase(m_job.JobType.ToString());
            if (m_job.CurrentTrafficArea == null)
                lblCurrentTrafficArea.Text = "Unknown";
            else
                lblCurrentTrafficArea.Text = m_job.CurrentTrafficArea.TrafficAreaName;

            // Populate the Job Details fieldset.
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation client = facOrganisation.GetForIdentityId(m_job.IdentityId);
            lblLoadNumber.Text = m_job.LoadNumber;
            lblLoadNumberText.Text = client.LoadNumberText;
            if (m_job.JobType == eJobType.Return)
                lblReturnReferenceNumber.Text = m_job.ReturnReceiptNumber;
            else
                lblReturnReferenceNumber.Text = "N/A";
            int numberOfPallets = 0;
            
            foreach (Entities.Instruction instruction in m_job.Instructions)
                if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                    numberOfPallets = numberOfPallets + instruction.TotalPallets;
                        
            lblNumberOfPallets.Text = numberOfPallets.ToString();
            if (m_job.ForCancellation)
                lblMarkedForCancellation.Text = "Yes - " + m_job.ForCancellationReason;
            else
                lblMarkedForCancellation.Text = "No";

            if (m_job.PCVs.Count > 0)
                lblTakingPCVs.Text = "YES - INFORM DRIVER";
            else
                lblTakingPCVs.Text = "No";
        }

        private void LoadUnCommunicatedLegs()
        {
            // Populate the legs.
            Facade.IJob facJob = new Facade.Job();
            DataView dv = facJob.GetTrafficSheetForJobId(m_jobId).Tables[1].DefaultView;
            dv.RowFilter = "InstructionStateId = 2";

            dgTrafficSheet.DataSource = dv;
            dgTrafficSheet.DataBind();
        }

        #region Button Event Handlers





        #endregion

        #region DropDown Event Handlers

        protected void cboEditCommunicationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList cboEditCommunicationType = (DropDownList)sender;
            RequiredFieldValidator rfvEditText = (RequiredFieldValidator)cboEditCommunicationType.Parent.Parent.FindControl("rfvEditText");
            CustomValidator cfvEditText = (CustomValidator)cboEditCommunicationType.Parent.Parent.FindControl("cfvEditText");
            CheckBox chkResendText = (CheckBox)cboEditCommunicationType.Parent.Parent.FindControl("chkResendText");

            eDriverCommunicationType communicationType = (eDriverCommunicationType)Enum.Parse(typeof(eDriverCommunicationType), cboEditCommunicationType.SelectedValue.Replace(" ", ""));

            switch (communicationType)
            {
                case eDriverCommunicationType.Phone:
                    rfvEditText.Enabled = false;
                    cfvEditText.Enabled = false;
                    chkResendText.Visible = false;
                    break;
                case eDriverCommunicationType.Text:
                    rfvEditText.Enabled = true;
                    cfvEditText.Enabled = true;
                    chkResendText.Visible = true;
                    break;
                case eDriverCommunicationType.InPerson:
                    rfvEditText.Enabled = false;
                    cfvEditText.Enabled = false;
                    chkResendText.Visible = false;
                    break;
            }
        }



        #endregion

        #region DataGrid Event Handlers

        void gvCommunications_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Orchestrator.eDriverCommunicationType)(int)((DataRowView)e.Row.DataItem)["DriverCommunicationTypeId"]) != eDriverCommunicationType.Text)
                {
                    gvCommunications.Columns[5].Visible = false;
                }
            }
        }

        #endregion

        #region Object Data Source Events

        void odsCommunications_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {

        }

        void odsCommunications_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters.Add("jobId", m_jobId);
        }

        #endregion

        #region Dialog Window Events

        void dlgCommunicateThis_DialogCallBack(object sender, EventArgs e)
        {
            LoadJob();
            LoadUnCommunicatedLegs();
        }

        #endregion

        #region Validation Event Handlers

        protected void cfvSMSText_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = args.Value.Length <= 160;
        }

        #endregion
    }
}