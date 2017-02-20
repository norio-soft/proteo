using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using Telerik.Web.UI;
using System.Web.Services;

namespace Orchestrator.WebUI.Traffic
{
    public partial class changePlannedTimes : Orchestrator.Base.BasePage
    {
        protected int JobID
        {
            get { return int.Parse(Request.QueryString["jobId"]); }
        }

        private Entities.InstructionCollection _instructions = null;
        private Entities.InstructionCollection Instructions
        {
            get
            {
                if (_instructions == null)
                {
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    _instructions = facInstruction.GetForJobId(this.JobID);
                }

                return _instructions;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateLegTimes();

            infrigementDisplay.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            repLegs.ItemDataBound += new RepeaterItemEventHandler(repLegs_ItemDataBound);
        }

        void repLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.LegView leg = (Entities.LegView)e.Item.DataItem;

                RadDatePicker dteSDate = e.Item.FindControl("dteSDate") as RadDatePicker;
                RadTimePicker dteSTime = e.Item.FindControl("dteSTime") as RadTimePicker;

                RadDatePicker dteEDate = e.Item.FindControl("dteEDate") as RadDatePicker;
                RadTimePicker dteETime = e.Item.FindControl("dteETime") as RadTimePicker;

                DateTime startDateTime = leg.StartLegPoint.PlannedDateTime;
                DateTime endDateTime = leg.EndLegPoint.PlannedDateTime;

                dteSDate.SelectedDate = startDateTime;
                dteSTime.SelectedDate = startDateTime;

                dteEDate.SelectedDate = endDateTime;
                dteETime.SelectedDate = endDateTime;
            }
        }

        private Entities.InstructionCollection PopulateInstructions()
        {
            foreach (RepeaterItem item in repLegs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    int instructionID = int.Parse(((HtmlInputHidden)item.FindControl("hidInstructionID")).Value);

                    RadDatePicker dteSDate = item.FindControl("dteSDate") as RadDatePicker;
                    RadTimePicker dteSTime = item.FindControl("dteSTime") as RadTimePicker;

                    RadDatePicker dteEDate = item.FindControl("dteEDate") as RadDatePicker;
                    RadTimePicker dteETime = item.FindControl("dteETime") as RadTimePicker;

                    // Configure the new date and time values.
                    DateTime startDateTime = dteSDate.SelectedDate.Value;
                    startDateTime = startDateTime.Subtract(startDateTime.TimeOfDay);
                    startDateTime = startDateTime.Add(dteSTime.SelectedDate.Value.TimeOfDay);
                    DateTime endDateTime = dteEDate.SelectedDate.Value;
                    endDateTime = endDateTime.Subtract(endDateTime.TimeOfDay);
                    endDateTime = endDateTime.Add(dteETime.SelectedDate.Value.TimeOfDay);

                    // Locate the instruction(s) to alter.
                    Entities.Instruction instruction = Instructions.GetForInstructionId(instructionID);

                    if (Instructions.Count == 1)
                    {
                        // First (and only) instruction on the job, set the arrival and departure date time.
                        instruction.PlannedArrivalDateTime = startDateTime;
                        instruction.PlannedDepartureDateTime = endDateTime;
                    }
                    else
                    {
                        // Get the previous instruction.
                        Entities.Instruction previousInstruction = Instructions.GetPreviousInstruction(instruction);

                        Facade.IJob facJob = new Facade.Job();

                        if (previousInstruction != null)
                        {
                            if (previousInstruction.InstructionOrder == 0)
                            {
                                previousInstruction.PlannedArrivalDateTime = startDateTime;
                                var expectedTurnaroundTime = facJob.GetExpectedTurnaroundTimeForInstruction(previousInstruction);
                                previousInstruction.PlannedDepartureDateTime = previousInstruction.PlannedArrivalDateTime.Add(expectedTurnaroundTime);
                            }
                            else
                                previousInstruction.PlannedDepartureDateTime = startDateTime;
                        }

                        // Set the arrival date time for this instruction.
                        instruction.PlannedArrivalDateTime = endDateTime;
                        bool isLastInstruction = Instructions.IsLastInstruction(instruction);

                        if (isLastInstruction)
                        {
                            var expectedTurnaroundTime = facJob.GetExpectedTurnaroundTimeForInstruction(instruction);
                            instruction.PlannedDepartureDateTime = instruction.PlannedArrivalDateTime.Add(expectedTurnaroundTime);
                        }
                    }
                }
            }

            return Instructions;
        }

        private void PopulateLegTimes()
        {
            var instructions = this.Instructions;

            if (instructions != null)
            {
                var facInstruction = new Facade.Instruction();
                var legs = facInstruction.GetLegPlan(instructions, false).Legs();

                repLegs.DataSource = legs;
                repLegs.DataBind();

                if (Globals.Configuration.CollectionTimeDeliveryWindow)
                {
                    var collects = instructions.Where(i => i.InstructionTypeId == (int)eInstructionType.Load);

                    // Only set up collection time recalculation if the last collection point on the run has a delivery matrix
                    if (collects.Any() && collects.Last().Point != null && collects.Last().Point.DeliveryMatrix > 0)
                    {
                        var firstDropFound = false;

                        for (var i = 0; i < legs.Count; i++)
                        {
                            var leg = legs[i];
                            var legRow = (HtmlTableRow)repLegs.Items[i].FindControl("legRow");
                            var cssClass = legRow.Attributes["class"];

                            cssClass += string.Format(" legRow_{0}", leg.InstructionID);

                            if (leg.StartLegPoint.Instruction != null)
                                cssClass += string.Format(" legStart_{0}", leg.StartLegPoint.Instruction.InstructionID);

                            if (!firstDropFound && leg.EndLegPoint.InstructionTypeId == (int)eInstructionType.Drop)
                            {
                                firstDropFound = true;
                                cssClass += " firstDrop";
                            }

                            legRow.Attributes["class"] += cssClass.TrimStart();
                        }
                    }
                }
            }
            else
            {
                repLegs.DataSource = null;
                repLegs.DataBind();
            }
        }

        #region Button Events

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Facade.IJob facJob = new Facade.Job();
                Entities.InstructionCollection instructionCollection = PopulateInstructions();

                DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);
                Entities.FacadeResult result = facJob.UpdatePlannedTimes(this.JobID, instructionCollection, lastUpdateDate, ((Entities.CustomPrincipal)Page.User).Name);

                if (result.Success)
                {
                    // Successful update return and refresh the grid.
                    this.ReturnValue = "refresh";
                    this.Close();
                }
                else
                {
                    // Display infringements to user.
                    infrigementDisplay.Infringements = result.Infringements;
                    infrigementDisplay.DisplayInfringments();
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

		#region Page Methods

        [WebMethod]
        public static Facade.DeliveryWindow.CalculatedCollectionTimes RecalculateCollectionTimes(int jobID, DateTime earliestDropDateTime, int earliestDropPointID)
        {
            try
            {
                var facDeliveryWindow = new Facade.DeliveryWindow();
                return facDeliveryWindow.CalculateCollectionTime(jobID, earliestDropDateTime, earliestDropPointID);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

 		#endregion Page Methods

    }
}