using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Transactions;

namespace Orchestrator.WebUI.Traffic
{
    public partial class MultiTrunk : Orchestrator.Base.BasePage
    {
        private int _jobId = 0;
        private int _instructionId = 0;
        private DateTime _lastUpdateDate;
        private Entities.InstructionCollection _instructions = null;
        private Entities.Job _job = null;
        private Entities.Instruction _startInstruction = null;
        private Entities.Instruction _endInstruction = null;
        private Entities.CustomPrincipal user = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Entities.CustomPrincipal)Page.User;

            // Get the query string variables
            _jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            // If inserting a multi trunk inserts instructions between instruction 1 and 2 
            // then _instructionId represents instruction 2.
            _instructionId = Convert.ToInt32(Request.QueryString["iID"]);

            _lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            // Default the arrival date and departure dates to match the dates of their 
            // prior instructions respectively.
            GetJobAndInstructions();

            if (!Page.IsPostBack)
            {
                var busJob = new BusinessLogicLayer.Job();
                ucMultiTrunk.ArrivalDateTime = _startInstruction.PlannedDepartureDateTime.Add(busJob.GetMinimumTravellingTime());
            }
        }

        private void GetJobAndInstructions()
        {
            // Get the job.
            using (Facade.IInstruction facInstruction = new Facade.Instruction())
            {
                _endInstruction = facInstruction.GetInstruction(_instructionId);

                // Try and get the job from the cache
                _job = (Entities.Job)Cache.Get("JobEntityForJobId" + _jobId.ToString());

                if (_job == null)
                {
                    Facade.IJob facJob = new Facade.Job();
                    _job = facJob.GetJob(_jobId);

                    // Job was not in the cache thus get the instruction collection from the db
                    _instructions = new Facade.Instruction().GetForJobId(_jobId);

                    // Get the previous instruction
                    _startInstruction = _instructions.GetPreviousInstruction(_instructionId);
                }
                else
                {
                    // Job is in the cache, check for instructions

                    if (_job.Instructions != null)
                    {
                        // We have instructions
                        _instructions = _job.Instructions;

                        // use the instruction collection from the cached job
                        _startInstruction = _instructions.GetPreviousInstruction(_instructionId);
                    }
                    else
                    {
                        // otherwise get a fresh instruction collection
                        _instructions = new Facade.Instruction().GetForJobId(_jobId);

                        // Get the previous instruction
                        _startInstruction = _instructions.GetPreviousInstruction(_instructionId);
                    }
                }
            }
        }

        protected void btnConfirm_Click1(object sender, EventArgs e)
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.FacadeResult result = null;

            result = facJob.MultiTrunk(_jobId, ucMultiTrunk.SelectedMultiTrunk, _endInstruction, ucMultiTrunk.ArrivalDateTime, _job.IdentityId, user.UserName, _lastUpdateDate, ucMultiTrunk.UseDriver, ucMultiTrunk.UseVehicle);

            if (result.Success == false)
            {
                // Display infringements
                infrigementDisplay.Infringements = result.Infringements;
                infrigementDisplay.DisplayInfringments();
                return;
            }

            this.ReturnValue = "refresh";
            this.Close();

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
