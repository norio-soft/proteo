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
    public partial class addDestination : Orchestrator.Base.BasePage
    {
        private const string jobEntityForJobId = "JobEntityForJobId";
        private DateTime _minDate;
        private const string vs_JobId = "vs_JobId";
        private int JobId
        {
            get { return ViewState[vs_JobId] == null ? -1 : (int)ViewState[vs_JobId]; }
            set { ViewState[vs_JobId] = value; }
        }

        public string MinDate
        {
            get { return _minDate.ToString("yyyy-MM-dd HH:mm"); }
        }
        private Entities.Job CurrentJob
        {
            get
            {
                Entities.Job l_job = null;

                if(JobId > 0)
                {
                    l_job = (Entities.Job)Cache.Get(jobEntityForJobId + JobId);

                    if (l_job == null)
                    {
                        Facade.IJob facJob = new Facade.Job();
                        l_job = facJob.GetJob(JobId, true, true);

                        Cache.Add(jobEntityForJobId + JobId.ToString() ,
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
            else
                if (Page.IsCallback || Page.IsCallback)
                    pnlAddDestination.Visible = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //cboOrganisation.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            //cboPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);

            btnAddDestination.Click += new EventHandler(btnAddDestination_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        #region Private Methods

        private void AddDestination(int jobId)
        {
            DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            Facade.IJob facJob = new Facade.Job();

            // Define the trunk instruction.
            DateTime bookedDateTime = new DateTime(rdiArrivalDate.SelectedDate.Value.Year, rdiArrivalDate.SelectedDate.Value.Month, rdiArrivalDate.SelectedDate.Value.Day, rdiArrivalTime.SelectedDate.Value.Hour, rdiArrivalTime.SelectedDate.Value.Minute, 0);
            TimeSpan travellingTime = bookedDateTime.Subtract(CurrentJob.Instructions[CurrentJob.Instructions.Count - 1].PlannedDepartureDateTime);

            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;
            Entities.FacadeResult result = facJob.AttachDestination(CurrentJob, ucPoint.PointID, chkDriver.Checked, chkVehicle.Checked, chkTrailer.Checked, travellingTime, true, user.IdentityId, user.UserName);

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
        
        private string GetResponse()
        {
            string retVal = "<AddDestination />";
            return retVal;
        }

        private void PopulateStaticControls()
        {
            btnAddDestination.Visible = false;
            lblMessage.Text = "This job has been cancelled or invoiced and you are not able to add a destination at this time.";
            pnlAddDestination.Visible = false;

            int jobId = 0;
            if (int.TryParse(Request.QueryString["jobId"], out jobId))
            {
                JobId = jobId;

                // If the job is invoiced or cancelled we can not add a destination.
                if (CurrentJob.JobState != eJobState.Invoiced && CurrentJob.JobState != eJobState.Cancelled)
                {
                    Entities.Instruction endInstruction = CurrentJob.Instructions[CurrentJob.Instructions.Count - 1];
                    //rdiArrivalTime.MinDate = endInstruction.PlannedDepartureDateTime.Add(new TimeSpan(0, 15, 0));
                    _minDate = endInstruction.PlannedDepartureDateTime;
                    chkDriver.Enabled = endInstruction.Driver != null;
                    chkVehicle.Enabled = endInstruction.Vehicle != null;
                    chkTrailer.Enabled = endInstruction.Trailer != null;

                    if (endInstruction.Driver != null)
                        chkDriver.Text = endInstruction.Driver.Individual.FullName;
                    if (endInstruction.Vehicle != null)
                        chkVehicle.Text = endInstruction.Vehicle.RegNo;
                    if (endInstruction.Trailer != null)
                        chkTrailer.Text = endInstruction.Trailer.TrailerRef;

                    lblMessage.Text = "Please specify the destination point and arrival time, you may also supply the resources that will be assigned to the destination.";
                    pnlAddDestination.Visible = true;
                    btnAddDestination.Visible = true;
                }
            }
        }

        #endregion

        void btnAddDestination_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int jobId = 0;
                if (int.TryParse(Request.QueryString["jobId"], out jobId))
                    AddDestination(jobId);
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Cache.Remove(jobEntityForJobId + JobId.ToString());
            this.Close();
        }

    }
}