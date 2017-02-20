using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Job
{
    public partial class removeDestination : Orchestrator.Base.BasePage
    {
        private const string jobEntityForJobId = "JobEntityForJobId";
        private Entities.Job CurrentJob
        {
            get
            {
                Entities.Job l_job = null;

                if (JobId > 0)
                {
                    l_job = (Entities.Job)Cache.Get(jobEntityForJobId + JobId);

                    if (l_job == null)
                    {
                        Facade.IJob facJob = new Facade.Job();
                        l_job = facJob.GetJob(JobId, true, true);

                        Cache.Add(jobEntityForJobId + JobId.ToString(),
                        l_job,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(10),
                        CacheItemPriority.Normal,
                        null);
                    }
                }

                return l_job;
            }
        }

        private const string vs_FoundInstruction = "vs_FoundInstruction";
        private Entities.Instruction FoundInstruction
        {
            get { return ViewState[vs_FoundInstruction] == null ? null : (Entities.Instruction)ViewState[vs_FoundInstruction]; }
            set { ViewState[vs_FoundInstruction] = value; }
        }

        private const string vs_JobId = "vs_JobId";
        private int JobId
        {
            get { return ViewState[vs_JobId] == null ? -1 : (int)ViewState[vs_JobId]; }
            set { ViewState[vs_JobId] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnRemoveDestination.Click += new EventHandler(btnRemoveDestination_Click);
        }

        #region Private Methods

        private string GetResponse()
        {
            string retVal = "<RemoveDestination />";
            return retVal;
        }

        private void PopulateStaticControls()
        {
            btnRemoveDestination.Visible = false;
            lblMessage.Text = "There is no destination on this job that can be removed.";

            int jobId = 0;
            if (int.TryParse(Request.QueryString["jobId"], out jobId))
            {
                JobId = jobId;
                Facade.IJob facJob = new Facade.Job();

                // If the job is complete we can not remove the destination.
                if ((int)CurrentJob.JobState <= (int)eJobState.InProgress && CurrentJob.Instructions.Count > 0)
                {
                    FoundInstruction = CurrentJob.Instructions.Find(instruc => instruc.InstructionTypeId == (int)eInstructionType.Trunk && instruc.InstructionOrder == CurrentJob.Instructions.Count - 1 && (instruc.CollectDrops.Count == 0 || instruc.CollectDrops[0].OrderID == 0));

                    if (FoundInstruction != null)
                    {
                        lblMessage.Text = "This will remove the final destination from this job and the resources will be no longer be going to " + FoundInstruction.Point.Description + ".  Are you sure you wish to remove this destination?.";
                        btnRemoveDestination.Visible = true;
                    }
                }
            }
        }

        private void RemoveDestination(int jobId)
        {
            DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            Facade.IJob facJob = new Facade.Job();

            if (CurrentJob.Instructions.Count > 0 && FoundInstruction == null)
                FoundInstruction = CurrentJob.Instructions.Find(instruc => instruc.InstructionTypeId == (int)eInstructionType.Trunk && instruc.InstructionOrder == CurrentJob.Instructions.Count - 1 && (instruc.CollectDrops.Count == 0 || instruc.CollectDrops[0].OrderID == 0));

            Entities.FacadeResult result = facJob.RemoveInstruction(CurrentJob, FoundInstruction.InstructionID, ((Entities.CustomPrincipal)Page.User).UserName);

            if (result.Success)
            {
                Cache.Remove(jobEntityForJobId + JobId.ToString());
                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                infrigementDisplay.Infringements = result.Infringements;
                infrigementDisplay.DisplayInfringments();
            }
        }

        #endregion

        #region Event Handlers

        void btnRemoveDestination_Click(object sender, EventArgs e)
        {
            int jobId = 0;
            if (int.TryParse(Request.QueryString["jobId"], out jobId))
                RemoveDestination(jobId);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Cache.Remove(jobEntityForJobId + JobId.ToString());
            this.Close();
        }

        #endregion
    }
}