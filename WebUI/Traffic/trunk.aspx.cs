using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic
{
    public partial class Trunk : Orchestrator.Base.BasePage
    {
        #region Protected Fields
        protected int m_instructionId = 0;
        protected string m_driver = string.Empty;
        protected string m_vehicle = string.Empty;

        private string vs_isUpdate = "vs_isUpdate";
        protected bool IsUpdate
        {
            get { return ViewState[vs_isUpdate] == null ? false : (bool)ViewState[vs_isUpdate]; }
            set { ViewState[vs_isUpdate] = value; }
        }
        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            m_instructionId = int.Parse(Request.QueryString["iID"]);
            if (!IsPostBack && Request.QueryString["rcbID"] == null)
            {
                PopulateForm();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnTrunk.Click += new EventHandler(btnTrunk_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void btnTrunk_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                TrunkLeg();
        }

        private string GetResponse()
        {
            string retVal = "<TrunkLeg />";
            return retVal;
        }

        #endregion

        #region Page Setup
        private void PopulateForm()
        {
            if (Request.QueryString["Driver"] != "")
            {
                m_driver = Request.QueryString["Driver"];
                chkDriver.Text = m_driver;
            }
            else
                chkDriver.Enabled = false;

            if (Request.QueryString["RegNo"] != "")
            {
                m_vehicle = Request.QueryString["RegNo"];
                chkVehicle.Text = m_vehicle;
            }
            else
                chkVehicle.Enabled = false;

            if (!string.IsNullOrEmpty(Request.QueryString["IsUpdate"]))
                IsUpdate = bool.Parse(Request.QueryString["IsUpdate"]);

            Entities.Instruction previousInstruction = null;
            using (Facade.IInstruction facInstruction = new Facade.Instruction())
            {
                Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);
                Entities.Job job = null;

                // Try and get the job from the cache
                job = (Entities.Job)Cache.Get("JobEntityForJobId" + instruction.JobId.ToString());

                DateTime trunkStart = DateTime.Now;
                DateTime trunkEnd = DateTime.Now;

                if (IsUpdate)
                {
                    lblTrunk.Text = "Update Trunk Leg";
                    btnTrunk.Text = "Update";

                    #region Update Trunk

                    this.ucPoint.SelectedPoint = instruction.Point;

                    rdiStartDate.SelectedDate = instruction.PlannedArrivalDateTime;
                    rdiStartTime.SelectedDate = instruction.PlannedArrivalDateTime;

                    rdiEndDate.SelectedDate = instruction.PlannedDepartureDateTime;
                    rdiStartTime.SelectedDate = instruction.PlannedDepartureDateTime;

                    trResources.Visible = false;
                    rdiStartDate.Enabled = rdiStartTime.Enabled = false;
                    rdiEndDate.Enabled = rdiEndTime.Enabled = false;
                    #endregion
                }
                else
                {
                    #region New Trunk
                    if (job == null)
                    {
                        // Job was not in the cache thus get the instruction collection from the db
                        Entities.InstructionCollection instructions = new Facade.Instruction().GetForJobId(instruction.JobId, true);

                        // Get the previous instruction
                        previousInstruction = instructions.GetPreviousInstruction(instruction.InstructionID);
                    }
                    else
                    {
                        // Job is in the cache, check for instructions

                        if (job.Instructions != null)
                        {
                            // We have instructions

                            // use the instruction collection from the cached job
                            previousInstruction = job.Instructions.GetPreviousInstruction(instruction.InstructionID);
                        }
                        else
                        {
                            // otherwise get a fresh instruction collection
                            Entities.InstructionCollection instructions = new Facade.Instruction().GetForJobId(instruction.JobId, true);

                            // Get the previous instruction
                            previousInstruction = instructions.GetPreviousInstruction(instruction.InstructionID);
                        }
                    }

                    if (previousInstruction == null)
                        previousInstruction = instruction;

                    previousInstruction.PlannedDepartureDateTime = previousInstruction.PlannedDepartureDateTime.Subtract(new TimeSpan(0, 0, 0, previousInstruction.PlannedDepartureDateTime.Second, previousInstruction.PlannedDepartureDateTime.Millisecond));
                    instruction.PlannedArrivalDateTime = instruction.PlannedArrivalDateTime.Subtract(new TimeSpan(0, 0, 0, instruction.PlannedArrivalDateTime.Second, instruction.PlannedDepartureDateTime.Millisecond));

                    if (previousInstruction == instruction)
                    {
                        trunkStart = instruction.PlannedArrivalDateTime.AddHours(2);
                        trunkEnd = instruction.PlannedDepartureDateTime.Subtract(new TimeSpan(0, 2, 0, 0, 0));
                    }
                    else
                    {
                        trunkStart = previousInstruction.PlannedDepartureDateTime.AddHours(2);
                        trunkEnd = instruction.PlannedArrivalDateTime.Subtract(new TimeSpan(0, 2, 0, 0, 0));
                    }

                    trunkStart = trunkStart.AddSeconds(-trunkStart.Second);
                    trunkEnd = trunkEnd.AddSeconds(-trunkEnd.Second);

                    if (trunkStart >= trunkEnd)
                    {
                        if (previousInstruction == instruction)
                        {
                            trunkStart = instruction.PlannedArrivalDateTime.AddMinutes(15);
                            trunkEnd = instruction.PlannedDepartureDateTime.AddMinutes(-15);
                        }
                        else
                        {
                            trunkStart = previousInstruction.PlannedDepartureDateTime.AddMinutes(15);
                            trunkEnd = instruction.PlannedArrivalDateTime.AddMinutes(-15);
                        }

                        if (trunkStart >= trunkEnd)
                        {
                            if (previousInstruction == instruction)
                            {
                                trunkStart = instruction.PlannedArrivalDateTime.AddMinutes(1);
                                trunkEnd = instruction.PlannedDepartureDateTime.AddMinutes(-1);
                            }
                            else
                            {
                                trunkStart = previousInstruction.PlannedDepartureDateTime.AddMinutes(1);
                                trunkEnd = instruction.PlannedArrivalDateTime.AddMinutes(-1);
                            }
                        }
                    }

                    if (trunkStart == trunkEnd)
                    {
                        if (previousInstruction == instruction)
                        {
                            trunkStart = instruction.PlannedArrivalDateTime.AddMinutes(60);
                            trunkEnd = instruction.PlannedDepartureDateTime.AddMinutes(-60);
                        }
                        else
                        {
                            trunkStart = previousInstruction.PlannedDepartureDateTime.AddMinutes(60);
                            trunkEnd = instruction.PlannedArrivalDateTime.AddMinutes(-60);
                        }
                    }
                    #endregion
                }

                rdiStartDate.SelectedDate = trunkStart;
                rdiStartTime.SelectedDate = trunkStart;
                rdiEndDate.SelectedDate = trunkEnd;
                rdiEndTime.SelectedDate = trunkEnd;
            }
        }
        #endregion

        #region Trunking Methods
        private void TrunkLeg()
        {
            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;
            Entities.FacadeResult result = null;

            int pointId = ucPoint.PointID;// int.Parse(cboPoint.SelectedValue);

            if (!IsUpdate)
            {
                #region Create New Trunk
                Entities.Instruction instruction = null;
                using (Facade.IInstruction facInstruction = new Facade.Instruction())
                {
                    instruction = facInstruction.GetInstruction(m_instructionId);
                }

                // We've trunked the leg.
                DateTime arriveAtTrunkPoint;
                DateTime leaveTrunkPoint;

                // Get the arrival time.
                //arriveAtTrunkPoint = timeStartDate.xDateTime;
                arriveAtTrunkPoint = new DateTime(rdiStartDate.SelectedDate.Value.Year, rdiStartDate.SelectedDate.Value.Month, rdiStartDate.SelectedDate.Value.Day, rdiStartTime.SelectedDate.Value.Hour, rdiStartTime.SelectedDate.Value.Minute, 0);

                // Get the departure time.
                //leaveTrunkPoint = timeEndDate.xDateTime;
                leaveTrunkPoint = new DateTime(rdiEndDate.SelectedDate.Value.Year, rdiEndDate.SelectedDate.Value.Month, rdiEndDate.SelectedDate.Value.Day, rdiEndTime.SelectedDate.Value.Hour, rdiEndTime.SelectedDate.Value.Minute, 0);

                DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

                // Create the new trunk instruction
                Entities.Instruction trunkInstruction = new Orchestrator.Entities.Instruction();
                trunkInstruction.InstructionTypeId = (int)eInstructionType.Trunk;
                trunkInstruction.JobId = instruction.JobId;
                trunkInstruction.PointID = pointId;
                trunkInstruction.PlannedDepartureDateTime = leaveTrunkPoint;
                trunkInstruction.PlannedArrivalDateTime = arriveAtTrunkPoint;
                trunkInstruction.Trailer = instruction.Trailer;

                // Set the booked datetime of the trunk to the trunks planned arrival time. This could produce a situation 
                // where the booked times do not flow, however, the trunks booked time is irrelevant so if this happens, it shouldn't matter.
                // Failing to set a booked time for the trunk here would cause an error when attempting to save the instruction.
                trunkInstruction.BookedDateTime = arriveAtTrunkPoint;

                // Note: Always get the driver and the vehicle.
                trunkInstruction.Driver = instruction.Driver;
                trunkInstruction.Vehicle = instruction.Vehicle;

                // Add the trunk to the job.
                using (Facade.IJob facJob = new Facade.Job())
                {
                    Entities.Job job = facJob.GetJob(instruction.JobId);
                    result = facJob.Trunk(trunkInstruction, instruction, job.IdentityId, user.UserName, job.LastUpdateDate, chkDriver.Checked, chkVehicle.Checked);
                }

                #endregion
            }
            else
            {
                using (Facade.IInstruction facInstruction = new Facade.Instruction())
                {
                    facInstruction.UpdatePointID(m_instructionId, pointId, user.UserName);
                    result = new Orchestrator.Entities.FacadeResult(true);
                }
            }

            if (result.Success)
            {
                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                infringementDisplay.Infringements = result.Infringements;
                infringementDisplay.DisplayInfringments();
            }

        }
        #endregion
    }
}