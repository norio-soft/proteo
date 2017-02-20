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

namespace Orchestrator.WebUI.Job
{
    public partial class addMultiDestination : Orchestrator.Base.BasePage
    {
        private const string jobEntityForJobId = "JobEntityForJobId";

        private int _jobId = 0;
        private DateTime _lastUpdateDate;
        private Entities.InstructionCollection _instructions = null;
        private Entities.Job _job = null;
        private Entities.Instruction _endInstruction = null;
        private Entities.CustomPrincipal user = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Entities.CustomPrincipal)Page.User;

            // Get the query string variables
            _jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            _lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            // Default the arrival date and departure dates to match the dates of their 
            // prior instructions respectively.
            GetJobAndInstructions();

            if (!Page.IsPostBack)
            {
                var busJob = new BusinessLogicLayer.Job();
                ucMultiDestination.ArrivalDateTime = _endInstruction.PlannedDepartureDateTime.Add(busJob.GetMinimumTravellingTime());
            }
        }

        private void GetJobAndInstructions()
        {
            // Get the job.
            using (Facade.IInstruction facInstruction = new Facade.Instruction())
            {
                // Try and get the job from the cache
                _job = (Entities.Job)Cache.Get("JobEntityForJobId" + _jobId.ToString());

                if (_job == null)
                {
                    Facade.IJob facJob = new Facade.Job();
                    _job = facJob.GetJob(_jobId);

                    // Job was not in the cache thus get the instruction collection from the db
                    _instructions = new Facade.Instruction().GetForJobId(_jobId);
                }
                else
                {
                    // Job is in the cache, check for instructions

                    if (_job.Instructions != null)
                    {
                        // We have instructions
                        _instructions = _job.Instructions;
                    }
                    else
                    {
                        // otherwise get a fresh instruction collection
                        _instructions = new Facade.Instruction().GetForJobId(_jobId);
                    }
                }

                // Get the end instruction
                _endInstruction = _instructions.Find(instruc => instruc.InstructionOrder == _instructions.Count - 1);

                if (_endInstruction == null)
                    throw new ApplicationException("Cannot find last instruction.");
            }
        }

        protected void btnConfirm_Click1(object sender, EventArgs e)
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.FacadeResult result = null;

            result = facJob.AttachMultipleDestinations(_jobId, ucMultiDestination.SelectedMultiTrunk, _endInstruction, ucMultiDestination.ArrivalDateTime, _lastUpdateDate, ucMultiDestination.UseDriver, ucMultiDestination.UseVehicle, user.IdentityId, user.UserName);

            if (result.Success)
            {
                Cache.Remove(jobEntityForJobId + this._jobId.ToString());
                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                infrigementDisplay.Infringements = result.Infringements;
                infrigementDisplay.DisplayInfringments();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "refresh";
            this.Close();
        }
    }
}

